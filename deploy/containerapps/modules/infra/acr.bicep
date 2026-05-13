param location string
param userAssignedIdentityResourceId string

resource acr 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' = {
  name: 'ACR3destate'
  location: location
  sku: {
    name: 'Standard'
  }
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${userAssignedIdentityResourceId}': {}
    }
  }
  properties: {
    adminUserEnabled: false
  }
}
