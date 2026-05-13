param location string = resourceGroup().location
param environment string
param azureContainerRegistry string
param containerRegistryUserName string
@secure()
param containerRegistryPassword string 
@secure()
param serviceBusConnection string
param managedIdentityClientId string
param userAssignedIdentityName string 
param keyVaultName string 
param imageTag string
param environmentName string



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

module dbInteriorDesignersProducts '../infra/cosmos-collection.bicep' = {
  name: '${deployment().name}-infra-cosmos-image-api-collection'
  params: {
    environment: environment
    collectionName: 'images-metadata'
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
  name: 'images-management-api'
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
          name: 'images-management-api'
          image: '${azureContainerRegistry}/images-management.api:${imageTag}'
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
        maxReplicas: 20
        rules: [
          {
            name: 'topic-based-scaling'
            custom: {
              type: 'azure-servicebus'
              auth: [
                {
                  secretRef: 'servicebusref'
                  triggerParameter: 'connection'
                }
              ]              
              metadata: {
                topicName: 'resize-image-requested'
                subscriptionName: 'images-management-api'
                messageCount: '50'
              }
            }
          }
          {
            name: 'http-rule'
            http: {
                metadata: {
                  concurrentRequests: '100'
                }
               }
              }
        ]        
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
        {
          name: 'servicebusref'
          value: serviceBusConnection
        }    
      ]        
      activeRevisionsMode: 'single'
      dapr: {
        enabled: true
        appId: 'images-management-api'
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
