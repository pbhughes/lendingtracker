# Log in to Azure


# Variables
RESOURCE_GROUP="needthatback"
LOCATION="westus2"
ENV_NAME="lendingViewApiContainerEnv"
APP_NAME="lendingViewApipiContainerApp"
IMAGE_NAME="ubuntu-latest"
CONTAINER_PORT=443

# Create a resource group
az group create --name $RESOURCE_GROUP --location $LOCATION

# Create a container app environment
az containerapp env create `
    --name $ENV_NAME `
    --resource-group $RESOURCE_GROUP `
    --location $LOCATION

# Create a container app
az containerapp create `
    --name $APP_NAME `
    --resource-group $RESOURCE_GROUP `
    --environment $ENV_NAME `
    --image $IMAGE_NAME `
    --target-port $CONTAINER_PORT `
    --ingress external
