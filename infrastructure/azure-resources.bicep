// Parameters
param location string = resourceGroup().location
param storageAccountName string = 'indextablestorage'
param functionAppName string = 'indexTableFunctionApp'
param appInsightsName string = 'indexTableAppInsights'
param searchServiceName string = 'indexTableSearchService'
param searchServiceSku string = 'standard'

// Azure Storage Account
resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
  }
}

resource tableService 'Microsoft.Storage/storageAccounts/tableServices@2023-01-01' = {
  name: 'default'
  parent: storageAccount
}

resource table 'Microsoft.Storage/storageAccounts/tableServices/tables@2023-01-01' = {
  name: 'dataIndexTable'
  parent: tableService
  properties: {
    signedIdentifiers: [
      {
        id: 'unique-id'
        accessPolicy: {
          startTime: '2024-09-12T00:00:00Z'
          expiryTime: '2024-09-13T00:00:00Z'
          permission: 'r'
        }
      }
    ]
  }
}
// Azure Function App Plan
resource functionAppPlan 'Microsoft.Web/serverfarms@2022-03-01' = {
  name: 'indexTableFunctionAppPlan'
  location: location
  sku: {
    name: 'Y1'
    tier: 'Dynamic'
  }
}


// Application Insights
resource appInsights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

// Azure Function App
resource functionApp 'Microsoft.Web/sites@2022-03-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    serverFarmId: functionAppPlan.id
    siteConfig: {
      appSettings: [
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'AzureWebJobsStorage'
          value: storageAccount.properties.primaryEndpoints.table
        }
        {
          name: 'AzureCognitiveSearchEndpoint'
          value: 'https://{searchServiceName}.search.windows.net'
        }
        {
          name: 'AzureCognitiveSearchApiKey'
          value: searchService.listKeys().primaryKey
        }
      ]
    }
  }
}

// Azure Cognitive Search Service
resource searchService 'Microsoft.Search/searchServices@2024-06-01-preview' = {
  name: searchServiceName
  location: location
  sku: {
    name: searchServiceSku
  }
  properties: {
    partitionCount: 1
    replicaCount: 1
  }
}
