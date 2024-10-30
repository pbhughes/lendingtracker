$scriptUrlBase = 'https://raw.githubusercontent.com/Microsoft/sql-server-samples/master/samples/manage/azure-sql-db-managed-instance/attach-vpn-gateway'

$parameters = @{
  subscriptionId = 'a4a511b2-aaab-42a0-b7a5-103321c86780'
  resourceGroupName = 'needthatback'
  virtualNetworkName = 'vnet-free-sql-mi-2047952'
  certificateNamePrefix  = 'data'
  }

Invoke-Command -ScriptBlock ([Scriptblock]::Create((iwr ($scriptUrlBase+'/attachVPNGateway.ps1?t='+ [DateTime]::Now.Ticks)).Content)) -ArgumentList $parameters, $scriptUrlBase