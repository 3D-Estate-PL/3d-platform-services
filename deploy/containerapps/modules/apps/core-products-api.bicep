param location string = resourceGroup().location
param environment string
param azureContainerRegistry string
param containerRegistryUserName string
@secure()
param containerRegistryPassword string 
param managedIdentityClientId string
param userAssignedIdentityName string 
param keyVaultName string
param imageTag string
param environmentName string
@secure()
param serviceBusConnection string



////////////////////////////////////////////////////////////////////////////////
// Infrastructure 
////////////////////////////////////////////////////////////////////////////////


module database '../infra/cosmos-db.bicep' = {
  name: '${deployment().name}-infra-cosmos-db'
  params: {
    location: location
    environment: environment
  }
}

module dbProducts '../infra/cosmos-collection.bicep' = {
  name: '${deployment().name}-infra-cosmos-products-collection'
  params: {
    environment: environment
    collectionName: 'coreProducts-products'
  }
}

module dbDefaultStyleProducts '../infra/cosmos-collection.bicep' = {
  name: '${deployment().name}-infra-cosmos-default-products-collection'
  params: {
    environment: environment
    collectionName: 'coreProducts-styleDefaultProducts'
  }
}

////////////////////////////////////////////////////////////////////////////////
// Container apps
////////////////////////////////////////////////////////////////////////////////

resource containerAppEvn 'Microsoft.App/managedEnvironments@2022-03-01' existing = {
  name: 'cappenv-3dplatform-${environment}'
}

resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: userAssignedIdentityName 
}

resource otelConnector 'Microsoft.App/containerApps@2022-03-01' existing = {
  name: 'otel-collector' 
}


resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  dependsOn:[
    dbProducts
    dbDefaultStyleProducts
    
  ]
  name: 'core-products-api'
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identity.id}': {}
    }}
  properties: {
    managedEnvironmentId: containerAppEvn.id
    template: {
      containers: [
        {
          name: 'core-products-api'
          image: '${azureContainerRegistry}/core-products.api:${imageTag}'
          resources: {
            cpu: '0.5'
            memory: '1.0Gi'
          }
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: environmentName
            }
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://0.0.0.0:80'
            }
            {
              name: 'OtlGrpcEndpoint'
              value: 'https://${otelConnector.properties.configuration.ingress.fqdn}'
            }  
            {
              name: 'Environment'
              value: environment
            }
            {
              name: 'ServiceNamespace'
              value: '3dEstate'
            }   
            {
              name: 'ServiceVersion'
              value: '1.0'
            }
            {
              name: 'ManagedIdentityClientId'
              value: managedIdentityClientId
            }        
            {
              name: 'KeyVaultName'
              value: keyVaultName
            }                    
          ]
          probes: [
            {
              type: 'liveness'
              initialDelaySeconds: 15
              periodSeconds: 30
              failureThreshold: 3
              timeoutSeconds: 1
              httpGet: {
                port: 80
                path: '/healthz/liveness'
              }
            }
            {
              type: 'startup'
              timeoutSeconds: 7
              httpGet: {
                port: 80
                path: '/healthz/startup'
              }
            }
            {
             type: 'readiness'
             timeoutSeconds: 7
             failureThreshold: 3
             httpGet: {
               port: 80
               path: '/healthz/readiness'
             }
            }
          ]          
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
    configuration: {
      registries: [
        {
          passwordSecretRef: 'containerregistrypasswordref'
          server: azureContainerRegistry
          username: containerRegistryUserName
        }
      ]
      secrets: [
        {
          name: 'containerregistrypasswordref'
          value: containerRegistryPassword
        }
      ]        
      activeRevisionsMode: 'single'
      dapr: {
        enabled: true
        appId: 'core-products-api'
        appPort: 80
      }
      ingress: {
        external: true
        targetPort: 80
        allowInsecure: true
      }
    }
  }
}





////////////////////////////////////////////////////////////////////////////////
// Dapr components
////////////////////////////////////////////////////////////////////////////////





/*
module daprStateStore '../dapr/statestore.bicep' = {
  name: '${deployment().name}-dapr-statestore'
  params: {
    containerAppsEnvironmentName: containerAppsEnvironmentName
    cosmosDbName: dbProducts.outputs.cosmoDbName
    cosmosCollectionName:''
    cosmosUrl: dbProducts.outputs.cosmosUrl
    cosmosKey: dbProducts.outputs.cosmosKey
  }
}
*/
