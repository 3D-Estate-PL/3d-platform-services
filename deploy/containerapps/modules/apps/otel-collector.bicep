param location string
param containerAppsEnvironmentId string
param volumeName string
param mountPath string = '/etc/otelcol'
param storageNameMount string

resource containerApp 'Microsoft.App/containerApps@2022-03-01' = {
  name: 'otel-collector'
  location: location
  properties: {
    managedEnvironmentId: containerAppsEnvironmentId
    template: {
      containers: [
        {
          name: 'otel-collector'
          image: 'otel/opentelemetry-collector:latest'
          args: [
            '--config=/etc/otelcol/otel-config-test.yml'
          ]   
          volumeMounts: [
            {
              mountPath: mountPath
              volumeName: volumeName
            }
          ]                 
        }
      ]
      scale: {
        minReplicas: 0
        maxReplicas: 1
      }
      volumes: [
        {
           name: volumeName
           storageName: storageNameMount
           storageType: 'AzureFile'
        }
      ]  
    }
    configuration: {
      activeRevisionsMode: 'single'
      ingress: {
        external: true
        targetPort: 4317
        transport: 'http2'
      }
    }
  }
}

output fqdn string = containerApp.properties.configuration.ingress.fqdn
