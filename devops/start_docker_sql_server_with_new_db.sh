#!/bin/bash -e

saPassword="Secret_Passw0rd"
dbName="SampleDb"

if [ -z "$1" ]; then
  echo "ERROR! No path to a SQL script for creating DB schema. Provide as a parameter"
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
  # Note that we gotta add '--user root' for the `chmod` command
  docker exec -i --user root sql-server sh -c "chmod +x $pathToWaitForItInContainer && $pathToWaitForItInContainer localhost:1433 -t 30"
  echo "Wait 1s extra as even after the port is available, the server still may need a moment"
  sleep 1
fi
 
createDbSqlScriptInContainer="/home/CreateAndMigrateDataContext.sql"

echo "Copy '$createDbSqlScript' into the container"
docker cp "$createDbSqlScript" sql-server:"$createDbSqlScriptInContainer"

echo "Create new database"
docker exec -i sql-server /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $saPassword -d master -Q "CREATE DATABASE $dbName"
echo "Create schema in DB"
docker exec -i sql-server /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $saPassword -d $dbName -i "$createDbSqlScriptInContainer"

# Note: To get the environment variable to persist after the script has completed, use `source ./script.sh`
echo "Set 'ConnectionString' as environment variable"
export ConnectionString="Data Source=localhost;Initial Catalog=$dbName;User Id=sa;Password=$saPassword;Connection Timeout=30;TrustServerCertificate=true"