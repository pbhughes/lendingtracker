# Variables
$resourceGroupName = "needthatback"
$location = "WestUS2"
$communicationServiceName = "CommunicationService"
$smsPhoneNumberName = "CommunicationNumber"
$subscriptionId = "a4a511b2-aaab-42a0-b7a5-103321c86780"

# Step 1: Create a Resource Group (if it doesn't exist)
if (-not (Get-AzResourceGroup -Name $resourceGroupName -ErrorAction SilentlyContinue)) {
    Write-Host "Creating Resource Group..."
    New-AzResourceGroup -Name $resourceGroupName -Location $location
} else {
    Write-Host "Resource Group already exists."
}

# Step 2: Create Azure Communication Service
Write-Host "Creating Azure Communication Service..."

#New-AzCommunicationService -ResourceGroupName $resourceGroupName -Name $communicationServiceName -DataLocation UnitedStates -Location Global

az communication create --name $communicationServiceName --location "Global" --data-location "United States" --resource-group $resourceGroupName

Write-Host "Azure Communication Service created successfully."

