#!/bin/bash -e

saPassword="Secret_Passw0rd"
dbName="SampleDb"

if [ -z "$1" ]; then
  echo "Provide path to a SQL script for creating DB schema"
  exit 1
fi
createDbSqlScript="$1"
 
pathToWaitForIt=""
if [ -z "$2" ]; then
  echo "Path to 'wait-for-it.sh' not provided. Will wait timeout for SQL engine to start"
else
  pathToWaitForIt="$2"
fi

echo "Pull & launch the SQL server image"
docker run --name sql-server -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$saPassword" -e "MSSQL_PID=Express" -p 1433:1433 -d mcr.microsoft.com/mssql/server:latest

if [ -z "$pathToWaitForIt" ]; then
  echo "Wait 10s for the SQL Server to get started"
  sleep 10
else
  # Wait till the TCP port becomes available.
  pathToWaitForItInContainer="./bin/wait-for-it.sh" 
  echo "Copy '$pathToWaitForIt' into the container '$pathToWaitForItInContainer'"
  docker cp "$pathToWaitForIt" sql-server:"$pathToWaitForItInContainer"
  echo "Wait for the TCP port to become available (or 30s timeout)"
  docker exec -i --user root sql-server sh -c "chmod +x $pathToWaitForItInContainer && $pathToWaitForItInContainer localhost:1433 -t 30"
  sleep 1
fi
 
createDbSqlScriptInContainer="/home/CreateAndMigrateDataContext.sql"

echo "Copy '$createDbSqlScript' into the container"
docker cp "$createDbSqlScript" sql-server:"$createDbSqlScriptInContainer"

echo "Create new database"
docker exec -i sql-server /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $saPassword -d master -Q "CREATE DATABASE $dbName"
echo "Create schema in DB"
docker exec -i sql-server /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $saPassword -d $dbName -i "$createDbSqlScriptInContainer"
