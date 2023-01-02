# Demo project: DbSample

**Example of EntityFramework-based back-end + automated tests against SQL Server in DevOps**

![CI](https://github.com/AKlaus/DbSample/actions/workflows/build_test.yml/badge.svg?branch=main)
[![Test Coverage](https://coveralls.io/repos/github/AKlaus/DbSample/badge.svg?branch=main)](https://coveralls.io/github/AKlaus/DbSample?branch=main)

Project shows examples of:
 - Automated DB tests for CRUD operations. 
 - A build pipeline to execute tests against an SQL Server instance.

See "[Pain & Gain of automated tests against SQL (MS SQL or PostgreSQL)](https://alex-klaus.com/dotnet-sql-tests/)" article for a deeper explanation of the solution.

## Overview of the solution

### Solution structure

| Project | Description                                                                                                                                                            |
|--------------|------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Database     | Database entities, EF DB context and migration.                                                                                                                        |
| Domain       | Queries and commands, domain layer logic with [Command Query Responsibility Segregation](https://martinfowler.com/bliki/CQRS.html) (CQRS) implementation. |
| Domain.Tests | Automated tests for querying and updating the entities.                                                                                                                |
| Api          | The application layer (API).                                                                                                                                           |

### Technologies

 - Main project: 
   - [.NET 7](https://docs.microsoft.com/en-us/dotnet/core/whats-new/dotnet-7);
   - [Entity Framework Core 7](https://docs.microsoft.com/en-us/ef/core/) and [dotnet-ef](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) CLI.
 - Test project:
   - [xUnit](https://xunit.net/) + [Respawn](https://github.com/jbogard/Respawn);
   - [Docker](https://www.docker.com/) + [SQL Server image](https://hub.docker.com/_/microsoft-mssql-server).

## Getting Started (locally)

Firstly, check out this Git repo and install dependencies:
 - [.NET SDK](https://dotnet.microsoft.com/download) v7.x;
 - [dotnet-ef](https://docs.microsoft.com/en-us/ef/core/cli/dotnet) CLI;
 - [Docker](https://www.docker.com/).

### Prepare SQL Server database

1. Generate SQL script to create the DB schema (see more info in ['./Database'](./Database/) folder)
```bash
dotnet ef migrations script -i -o CreateOrMigrateDatabase.sql --project Database/Database.csproj --startup-project Api/Api.csproj --context DataContext -v
```
2. Execute a `bash` script that
   1. launches SQL server in Docker container;
   2. creates a new database;
   3. populates the DB with the schema from the provided script;
   4. sets an environment variable with SQL connection string for consuming in the tests.
```bash
source ./devops/start_docker_sql_server_with_new_db.sh CreateOrMigrateDatabase.sql
```

*NOTE* that `start_docker_sql_server_with_new_db.sh` specifies the database name and _sa_ password, and also adds an environment variable named `ConnectionString` with the connection string. <br>
Alternatively, you can maintain the connection string in [testsettings.json](./Domain.Tests/testsettings.json).


Here you go. The SQL Server with an empty database is available.

### Run tests

To run the tests, make sure that correct connection string is specified in [testsettings.json](./Domain.Tests/testsettings.json) file or environment variables.  

Open the solution and run the tests from `Domain.Tests` project in your favourite IDE or via a command like
```bash
dotnet test --verbosity normal
```

### Run Web API (Swagger)

To run the application, the connection string in [appsettings.json](./Api/appsettings.json) file has to point to the right instance of the database.

Open and launch the solution (the `Api` project). It should open `https://localhost:7135/swagger` in the browser.

## Running tests in DevOps

See an example build pipeline that runs tests in GitHub Actions – [build_test.yml](./.github/workflows/build_test.yml).
