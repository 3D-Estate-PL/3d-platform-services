param location string = resourceGroup().location
param userAssignedIdentityName string 
param environment string
param storageAccountName string 
param applicationName string 


var storageNameMount = 'configmount' 
var volumeName = 'config'
var shareName  = 'collector-config'
var defaultTags = {
  environment: environment
}

////////////////////////////////////////////////////////////////////////////////
// Infrastructure
////////////////////////////////////////////////////////////////////////////////

module userAssignedManageIdentity 'modules/infra/manage-identity.bicep' = {
  scope: resourceGroup()
  name: '${environment}-infra-user-assigned-identity'
  params: {
    userAssignedIdentityName: userAssignedIdentityName
    location: location
  }
}


module storage 'modules/infra/storage.bicep' = {
  scope: resourceGroup()
  name: '${environment}-infra-storage'
  params: {
    storageAccountName: storageAccountName
    location: location
    applicationName: applicationName
    containerName: applicationName
    shareName: shareName
    resourceTags: defaultTags
  }
}

module containerAppsEnvironment 'modules/infra/container-apps-env.bicep' = {
  name: '${environment}-infra-container-app-env'
  params: {
    location: location
    environment: environment
  }
}

module environmentStorages 'modules/infra/acaEnvironmentStorages.bicep' = {
  scope: resourceGroup()
  name: '${environment}-infra-acaenvironmentstorages'
  dependsOn:[
    containerAppsEnvironment
  ]
  params: {
    acaEnvironmentName: containerAppsEnvironment.outputs.name
    storageAccountResName: storage.outputs.storageAccountName
    storageAccountResourceKey: storage.outputs.storageKey
    storageNameMount: storageNameMount
    shareName: shareName
  }
}


module serviceBus 'modules/infra/service-bus.bicep' = {
  name: '${environment}-infra-service-bus'
  params: {
    location: location
    environment: environment
  }
}


////////////////////////////////////////////////////////////////////////////////
// Dapr components
////////////////////////////////////////////////////////////////////////////////

module daprPubSub 'modules/dapr/pubsub.bicep' = {
  name: '${environment}-dapr-pubsub'
  params: {
    containerAppsEnvironmentName: containerAppsEnvironment.outputs.name
    serviceBusConnectionString: serviceBus.outputs.connectionString
  }
}


////////////////////////////////////////////////////////////////////////////////
// Container apps
////////////////////////////////////////////////////////////////////////////////


module otel_connector 'modules/apps/otel-collector.bicep' = {
  name: '${environment}-app-otel-collector'
  dependsOn: [
    containerAppsEnvironment
  ]
  params: {
    location: location
    containerAppsEnvironmentId: containerAppsEnvironment.outputs.id
    storageNameMount:storageNameMount
    volumeName:volumeName
  
  }
}
