$templateFile="template.json"
$parameterFile = "parameters.json"
az deployment group create --name deployVPN --resource-group needthatback --template-file $templateFile --parameters $parameterFile