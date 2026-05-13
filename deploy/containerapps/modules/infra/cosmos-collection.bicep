param environment string
param cosmosAccountName string = 'cosmodb-3dplatform-${environment}'
param cosmosDbName string = 'platform-db'
param collectionName string 

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2021-04-15' existing = {
  name: cosmosAccountName
}

resource cosmosDb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2021-04-15' = {
  parent: cosmosAccount
  name: cosmosDbName
  properties: {
    resource: {
      id: cosmosDbName
    }
  }
}

resource cosmosCollection 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2021-04-15' = {
  parent: cosmosDb
  name: collectionName
  properties: {
    resource: {
      id: collectionName
      partitionKey: {
        paths: [
          '/partitionKey'
        ]
        kind: 'Hash'
      }
    }
  }
}


output cosmosUrl string = cosmosAccount.properties.documentEndpoint
output cosmosKey string = cosmosAccount.listKeys().primaryMasterKey
output cosmosConnectionString string =  cosmosAccount.listConnectionStrings().connectionStrings[0].connectionString

// core products
output cosmosCollectionName string = cosmosCollection.name
output cosmoDbName string = cosmosDbName
