## EntityFramework commands cheat sheet

As the solution has separated the database concerns from the start-up project. 
Hence the `dotnet ef migrations` commands need explicitly specified reference to the start-up project (`--startup-project`) and the project with the DB context and entities (`--project`).

See the official [docs](https://docs.microsoft.com/en-us/ef/core/cli/dotnet#dotnet-ef-migrations-script) for the supported command arguments.

### Generate initial migration to create the schema

```bash
dotnet ef migrations add InititalCreate --project Database/Database.csproj --startup-project Api/Api.csproj --context DataContext -v
```
It adds `Migrations` folder that might require adding some EF Core NuGet packages to `Database` project. 

### Generate a full SQL script with all updates
The following command creates an SQL script that can be used for creating a new DB and updating an existing one.
```bash
dotnet ef migrations script -i -o CreateOrMigrateDatabase.sql --project Database/Database.csproj --startup-project Api/Api.csproj --context DataContext -v
```