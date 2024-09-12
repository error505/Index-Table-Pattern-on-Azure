# Index Table Function

This Azure Function is part of the **Index Table Pattern** architecture on Azure. It is designed to handle data updates by storing data in **Azure Table Storage** and indexing the data in **Azure Cognitive Search** for efficient search capabilities.

## 📑 Overview

The **Index Table Function** performs the following actions:

1. **Receives Data Update Requests**: The function receives HTTP POST requests containing JSON data.
2. **Updates Azure Table Storage**: The function updates the data in Azure Table Storage, using it as a scalable NoSQL store.
3. **Indexes Data in Azure Cognitive Search**: After updating Azure Table Storage, the function indexes the data in Azure Cognitive Search to enable fast and efficient search capabilities.
4. **Performs Search Requests**: The function receives HTTP GET requests with search queries and returns matching results from Azure Cognitive Search.

## 🛠️ Configuration

Ensure the following environment variables are set in your Azure Function App configuration:

- **`AzureWebJobsStorage`**: Connection string for the Azure Storage Account used for Table Storage.
- **`AzureCognitiveSearchEndpoint`**: The endpoint URL for your Azure Cognitive Search service (e.g., `https://<search-service-name>.search.windows.net`).
- **`AzureCognitiveSearchApiKey`**: The API key for your Azure Cognitive Search service.

## 🔧 Prerequisites

- Azure Function Core Tools (if running locally)
- .NET 8 SDK
- Azure CLI (for deployment)

## 🧪 How to Test the Function

You can test the function locally or on Azure using **Postman**.

### Testing `UpdateData` Locally

1. **Run the Azure Function Locally**:
    - Open a terminal or command prompt and run:

      ```bash
      func start
      ```

    - The function will start at `http://localhost:7071`.

2. **Open Postman**:
    - Create a new **POST** request.

3. **Configure the Postman Request**:
    - **Method**: `POST`
    - **URL**: `http://localhost:7071/api/update`
    - **Headers**:
      - **Content-Type**: `application/json`
    - **Body**:
      - Select **Raw** and choose **JSON** format.
      - Example JSON data:

        ```json
        {
          "PartitionKey": "SamplePartition",
          "RowKey": "SampleRow",
          "Name": "John Doe",
          "Email": "john.doe@example.com"
        }
        ```

4. **Send the Request**:
    - Click **Send** to execute the request.

5. **Check the Response**:
    - You should receive a response with `200 OK` and a message indicating that the data was updated and indexed successfully.

### Testing `SearchData` Locally

1. **Open Postman**:
    - Create a new **GET** request.

2. **Configure the Postman Request**:
    - **Method**: `GET`
    - **URL**: `http://localhost:7071/api/search?search=John Doe` (replace `John Doe` with the desired search query)

3. **Send the Request**:
    - Click **Send** to execute the request.

4. **Check the Response**:
    - You should receive a response with `200 OK` and the search results in JSON format.

### Testing on Azure

1. **Deploy the Azure Function**:
   - Deploy the function to Azure using the provided GitHub Actions workflow.

2. **Open Postman**:
    - Create a **POST** or **GET** request, depending on the function you are testing.

3. **Configure the Postman Request**:
    - **URL for `UpdateData`**: `https://<your-function-app-name>.azurewebsites.net/api/update`
    - **URL for `SearchData`**: `https://<your-function-app-name>.azurewebsites.net/api/search?search=John Doe`

4. **Include Function Key (If Required)**:
   - If your function requires a function key:
     - Add it as a query parameter or header as described earlier.

5. **Send the Request** and **Check the Response**.

## 🔍 Key Points to Remember

- **Ensure Environment Variables**: Make sure all required environment variables are properly set.
- **Check Azure Resources**: Confirm that Azure Table Storage and Azure Cognitive Search are correctly configured.
- **Monitor Logs**: Use Azure Monitor or local logs to debug and trace function execution.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

## 🙌 Contributions

Contributions are welcome! Please open an issue or submit a pull request for any improvements or suggestions.
