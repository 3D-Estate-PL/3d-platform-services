param location string = resourceGroup().location
param environment string
param azureContainerRegistry string
param containerRegistryUserName string
@secure()
param containerRegistryPassword string 
param imageTag string
param managedIdentityClientId string
param userAssignedIdentityName string 
param keyVaultName string
param environmentName string
@secure()
param serviceBusConnection string



var domainName = {
  'staging': 'platform-api-stg.3destate.pl'
  'prd': 'platform-api.3destate.pl'
}[environment]


////////////////////////////////////////////////////////////////////////////////
// Infrastructure 
////////////////////////////////////////////////////////////////////////////////


////////////////////////////////////////////////////////////////////////////////
// Container apps
////////////////////////////////////////////////////////////////////////////////


resource containerAppEvn 'Microsoft.App/managedEnvironments@2022-03-01' existing = {
  name: 'cappenv-3dplatform-${environment}'
}

resource envCertificate 'Microsoft.App/managedEnvironments/certificates@2022-10-01'  existing = {
  name: '3d-estate-cert'
  parent: containerAppEvn
}


resource otelConnector 'Microsoft.App/containerApps@2022-03-01' existing = {
  name: 'otel-collector' 
}

resource containerApp 'Microsoft.App/containerApps@2022-10-01' = {
  name: 'platform-api-gw'
  location: location
  properties: {
    managedEnvironmentId: containerAppEvn.id
    template: {
      containers: [
        {
          name: 'platform-api-gw'
          image: '${azureContainerRegistry}/platform-api-gw:${imageTag}'
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
              name: 'environment'
              value: environment
            }                      
          ]
        }
      ]
      scale: {
        minReplicas: 0
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
        appId: 'platform-api-gw'
        appPort: 80
      }
      ingress: {
        external: true
        targetPort: 80
        allowInsecure: false
        corsPolicy: {
          allowedOrigins: ['*']
          allowedHeaders: ['*']
          allowedMethods: ['*']
       }
       customDomains: [
        {
          bindingType: 'SniEnabled'
          certificateId: envCertificate.id
          name: domainName
        }    
      ]       
      }
    }
  }
}
