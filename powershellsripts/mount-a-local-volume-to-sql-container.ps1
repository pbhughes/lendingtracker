docker volume create lendingview-sql-data

docker stop sqlserver

docker run -e "ACCEPT_EULA=Y" -e "sa=Reguxalo26@" `
  -p 1433:1433 `
  -v lending-view-sql-data:/var/opt/mssql/data `
  --name sqlserver_two `
  mcr.microsoft.com/mssql/server:2022-latest

