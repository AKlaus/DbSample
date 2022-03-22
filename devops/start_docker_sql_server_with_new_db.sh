#!/bin/bash -e

if [ -z "$1" ]; then
    echo "Provide path to a SQL script for creating DB schema"
    exit 1
fi

createDbSqlScript="$1" 
saPassword="Secret_Passw0rd"
dbName="SampleDb"

echo "Pull & launch the SQL server image"
docker run --name sql-server -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=$saPassword" -e "MSSQL_PID=Express" -p 1433:1433 -d mcr.microsoft.com/mssql/server:latest
 
echo "Copy $createDbSqlScript into the container"
docker cp "$createDbSqlScript" sql-server:/home/CreateAndMigrateDataContext.sql

echo "Wait for the SQL Server to get started"
sleep 10

echo "Create new database"
docker exec -i sql-server /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $saPassword -d master -Q "CREATE DATABASE $dbName"
echo "Create schema in DB"
docker exec -i sql-server /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P $saPassword -d $dbName -i /home/CreateAndMigrateDataContext.sql
