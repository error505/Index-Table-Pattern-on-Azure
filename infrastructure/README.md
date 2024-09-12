# Infrastructure Deployment for Index Table Pattern

This folder contains the **Bicep template** and GitHub Actions workflow for deploying the necessary Azure resources for the **Index Table Pattern** on Azure. This architecture pattern uses Azure Table Storage for storing data and Azure AI Search for indexing and searching the data efficiently.

## 📑 Overview of Services

The following Azure services are deployed by the Bicep template:

1. **Azure Storage Account**: Provides scalable, low-cost storage for Table Storage.
2. **Azure Table Storage**: A NoSQL storage service for storing structured data.
3. **Azure Function App**: Hosts the Azure Function that handles data updates and triggers indexing in Azure AI Search.
4. **Azure AI Search**: A fully managed search-as-a-service solution to build rich search experiences over your data.
5. **Application Insights**: A monitoring service to collect telemetry data from the Azure Function.

### 🚀 Deployed Resources

1. **Azure Storage Account**:
   - **Purpose**: Provides the underlying storage for Azure Table Storage.
   - **Configuration**: 
     - **Type**: Standard Locally Redundant Storage (LRS)
     - **Access Tier**: Hot
     - **Minimum TLS Version**: TLS 1.2

2. **Azure Table Storage**:
   - **Purpose**: Stores structured data in a NoSQL format.
   - **Configuration**:
     - **Table Name**: `dataIndexTable`
     - **Partitioning**: Uses `PartitionKey` and `RowKey` for data organization.

3. **Azure Function App**:
   - **Purpose**: Hosts the function that updates Azure Table Storage and triggers indexing in Azure AI Search.
   - **Configuration**:
     - **Runtime**: .NET 8 (Isolated Mode)
     - **Plan**: Consumption Plan (Y1 Dynamic)
     - **Application Settings**:
       - **`FUNCTIONS_WORKER_RUNTIME`**: `dotnet`
       - **`APPINSIGHTS_INSTRUMENTATIONKEY`**: Instrumentation Key from Application Insights
       - **`AzureWebJobsStorage`**: Connection string for Azure Storage Account
       - **`AzureAISearchEndpoint`**: Endpoint for Azure AI Search
       - **`AzureAISearchApiKey`**: API key for Azure AI Search

4. **Azure AI Search**:
   - **Purpose**: Provides search and indexing capabilities for data stored in Azure Table Storage.
   - **Configuration**:
     - **SKU**: Standard (can be adjusted as needed)
     - **Partition Count**: 1
     - **Replica Count**: 1

5. **Application Insights**:
   - **Purpose**: Collects telemetry data from the Azure Function for monitoring and diagnostics.
   - **Configuration**:
     - **Retention**: 30 days
     - **Application Type**: Web

## 📂 Files in This Folder

- **`azure-resources.bicep`**: Bicep template file defining all the necessary Azure resources.
- **`.github/workflows/deploy-bicep.yml`**: GitHub Actions workflow to automate the deployment of the Azure infrastructure.

## 🛠️ How to Deploy the Infrastructure

### Prerequisites

1. **Azure Subscription**: An active Azure account with appropriate permissions.
2. **GitHub Repository**: Fork or create a repository to store your code and workflows.
3. **Azure CLI**: Installed and configured to manage Azure resources.

### Steps to Deploy

1. **Add Required Secrets to GitHub**:
   - Go to your repository’s **Settings > Secrets and variables > Actions > New repository secret**.
   - Add the following secrets:
     - **`AZURE_CLIENT_ID`**: Your Azure service principal client ID.
     - **`AZURE_CLIENT_SECRET`**: Your Azure service principal client secret.
     - **`AZURE_TENANT_ID`**: Your Azure tenant ID.

2. **Run the GitHub Action**:
   - The GitHub Actions workflow will automatically trigger on a push to the `main` branch or can be manually triggered.
   - To manually trigger, go to the **Actions** tab in your GitHub repository and select **Deploy Azure Infrastructure with Bicep**.

3. **Monitor the Deployment**:
   - Go to the **Actions** tab in your GitHub repository.
   - Select the **Deploy Azure Infrastructure with Bicep** workflow to monitor the deployment progress.
   - The workflow will use the Bicep template to deploy the resources to your Azure subscription.

## 🔍 Verification After Deployment

1. **Azure Portal**:
   - Go to the [Azure Portal](https://portal.azure.com/) and verify that the following resources are deployed:
     - **Storage Account** with Table Storage named `dataIndexTable`.
     - **Function App** named `indexTableFunctionApp`.
     - **Azure AI Search** service.
     - **Application Insights** for monitoring.

2. **Application Insights**:
   - Check the Application Insights resource for telemetry data to ensure that it’s properly configured and collecting data from your Azure Function.

## 📊 Post-Deployment Configuration

- **Verify Environment Variables**:
  - Ensure that all required environment variables (`AzureWebJobsStorage`, `AzureAISearchEndpoint`, `AzureAISearchApiKey`) are set correctly in your Azure Function App.
- **Run Tests**:
  - Follow the instructions in the [Function README](../azure-functions/README.md) to test the deployed function using Postman or any other HTTP client.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

## 🙌 Contributions

Contributions are welcome! Please open an issue or submit a pull request for any improvements or suggestions.
