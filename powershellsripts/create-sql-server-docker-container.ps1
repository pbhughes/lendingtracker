#docker network create lendingviewnetwork

docker run -e "ACCEPT_EULA=Y" `
   --network lendingviewnetwork `
   -e "MSSQL_SA_PASSWORD=Reguxalo26@" `
   -e "MSSQL_PID=Developer" `
   -p 1433:1433 `
   --name sqlserver2 `
   -d mcr.microsoft.com/mssql/server:2022-latest
