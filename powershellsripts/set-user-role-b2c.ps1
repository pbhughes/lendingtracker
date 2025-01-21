#variables

$userPrincipalName = "a783294f-b2cd-4a71-aa75-b88a1969ace1@needthatback.onmicrosoft.com"
$mailNickName = "a783294f-b2cd-4a71-aa75-b88a1969ace1"
$propertyName = "exention_fd654ae5-bf03-4677-9427-5fd01c8b4458_userRole"
$propertyValue = "admin"

az login


Install-Module -Name Microsoft.Graph -Scope CurrentUser
Connect-MgGraph -Scopes "User.ReadWrite.All"

Update-MgUser -UserId $userPrincipalName  -AdditionalProperties @{$propertyName = $propertyValue}

Write-Output "Extension property '$PropertyName' set for user '$UserId'."
