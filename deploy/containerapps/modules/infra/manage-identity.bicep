param userAssignedIdentityName string 
param location string


resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: userAssignedIdentityName
  location: location 
}



output userAssignedManageIdentityId string = identity.id
