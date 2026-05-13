param location string
param environment string
param cosmosAccountName string = 'cosmodb-3dplatform-${environment}'
param cosmosDbName string = 'platform-db'

resource cosmosAccount 'Microsoft.DocumentDB/databaseAccounts@2021-04-15' = {
  name: cosmosAccountName
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        locationName: location
      }
    ]
    capabilities: [
      {
        name: 'EnableServerless'
      }
    ]
  }
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



output cosmosUrl string = cosmosAccount.properties.documentEndpoint
output cosmosKey string = cosmosAccount.listKeys().primaryMasterKey
output cosmosConnectionString string =  cosmosAccount.listConnectionStrings().connectionStrings[0].connectionString

output cosmoDbName string = cosmosDbName
