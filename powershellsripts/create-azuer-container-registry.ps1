# Variables
$RESOURCE_GROUP="needthatback"
$LOCATION="West US 2" # e.g., eastus, westus, etc.
$REGISTRY_NAME="lendingviewacr" # Must be globally unique and between 5-50 characters.

# Create a Resource Group (if it doesn't exist)
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create the Azure Container Registry
az acr create `
  --resource-group $RESOURCE_GROUP `
  --name $REGISTRY_NAME `
  --sku Basic `
  --location $LOCATION

# Verify the ACR creation
az acr show --name $REGISTRY_NAME --resource-group $RESOURCE_GROUP --query "{Name:name, Status:provisioningState}" -o table
