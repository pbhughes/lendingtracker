az containerapp update `
  --name api-lending-tracker `
  --resource-group needthatback `
  --set-env-vars APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=34e61473-fb29-43a9-90c6-b5324284ff96;IngestionEndpoint=https://westus2-2.in.applicationinsights.azure.com/;LiveEndpoint=https://westus2.livediagnostics.monitor.azure.com/;ApplicationId=a1daa383-6400-4e5b-828b-c5ebd3060666"
