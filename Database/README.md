## EntityFramework commands cheat sheet

### Generate initial migration to create the schema 
```bash
dotnet ef migrations add InititalCreate --project Database/Database.csproj --startup-project Api/Api.csproj --context DataContext -v
```
It'll add `Migrations` folder that might require adding some EF Core NuGet packages

### Generate a full SQL script with all updates
```bash
# Get full script for creation and migration in `CreateOrMigrateDatabase.sql`:
dotnet ef migrations script -i -o CreateOrMigrateDatabase.sql --project Database/Database.csproj --startup-project Api/Api.csproj --context DataContext -v
```