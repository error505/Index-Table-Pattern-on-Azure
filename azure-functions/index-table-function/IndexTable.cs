using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using Azure.Search.Documents;
using Azure;
using Newtonsoft.Json;
using Azure.Search.Documents.Models;

namespace index_table_function
{
    public class IndexTableFunction
    {
        private readonly ILogger<IndexTableFunction> _logger;
        private readonly CloudTableClient _tableClient;
        private readonly SearchClient _searchClient;
        private readonly string _tableName = "dataIndexTable";

        public IndexTableFunction(ILogger<IndexTableFunction> logger)
        {
            _logger = logger;

            string storageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage")
                ?? throw new ArgumentNullException(nameof(storageConnectionString), "Environment variable 'AzureWebJobsStorage' is not set.");
            string searchEndpoint = Environment.GetEnvironmentVariable("AzureAISearchEndpoint")
                ?? throw new ArgumentNullException(nameof(searchEndpoint), "Environment variable 'AzureAISearchEndpoint' is not set.");
            string searchApiKey = Environment.GetEnvironmentVariable("AzureAISearchApiKey")
                ?? throw new ArgumentNullException(nameof(searchApiKey), "Environment variable 'AzureAISearchApiKey' is not set.");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(storageConnectionString);
            _tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            _searchClient = new SearchClient(new Uri(searchEndpoint), "my-index", new AzureKeyCredential(searchApiKey));
        }

        [Function("UpdateData")]
        public async Task<HttpResponseData> UpdateData([HttpTrigger(AuthorizationLevel.Function, "post", Route = "update")] HttpRequestData req)
        {
            _logger.LogInformation("Received data update request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            dynamic? data = JsonConvert.DeserializeObject(requestBody);
            if (data == null)
            {
                var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid or empty data provided.");
                return badResponse;
            }

            try
            {
                // Update Azure Table Storage
                var table = _tableClient.GetTableReference(_tableName);
                var insertOrMergeOperation = TableOperation.InsertOrMerge(new DynamicTableEntity("PartitionKey", "RowKey", "*", data));
                await table.ExecuteAsync(insertOrMergeOperation);

                _logger.LogInformation("Data updated in Azure Table Storage.");

                // Index data in Azure AI Search
                var searchIndexData = new IndexDocumentsBatch<SearchDocument>();
                searchIndexData.Actions.Add(IndexDocumentsAction.Upload(new SearchDocument(data)));

                await _searchClient.IndexDocumentsAsync(searchIndexData);

                _logger.LogInformation("Data indexed in Azure AI Search.");

                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteStringAsync("Data updated and indexed successfully.");
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An error occurred while processing your request.");
                return errorResponse;
            }
        }

        [Function("SearchData")]
        public async Task<HttpResponseData> SearchData([HttpTrigger(AuthorizationLevel.Function, "get", Route = "search")] HttpRequestData req)
        {
            _logger.LogInformation("Received search request.");

            // Get the search query from query parameters
            string query = req.Url.Query.TrimStart('?');
            if (string.IsNullOrEmpty(query))
            {
                var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Query parameter is missing.");
                return badResponse;
            }

            try
            {
                // Perform the search using Azure Cognitive Search
                var options = new SearchOptions
                {
                    Size = 10,  // Limit the results to 10 items for this example
                    IncludeTotalCount = true
                };

                var searchResults = await _searchClient.SearchAsync<SearchDocument>(query, options);

                // Prepare the response
                var searchResponse = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await searchResponse.WriteStringAsync(JsonConvert.SerializeObject(searchResults.Value.GetResults()));
                return searchResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error performing search: {ex.Message}");
                var errorResponse = req.CreateResponse(System.Net.HttpStatusCode.InternalServerError);
                await errorResponse.WriteStringAsync("An error occurred while performing the search.");
                return errorResponse;
            }
        }
    }
}
