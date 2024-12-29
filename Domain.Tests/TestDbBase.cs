using AK.DbSample.Database;
using AK.DbSample.Database.Configuration;
using AK.DbSample.Database.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Respawn;
using Respawn.Graph;

using Xunit;
using Xunit.Abstractions;

#pragma warning disable CS8618

namespace AK.DbSample.Domain.Tests;

/// <summary>
///		Base test class with a DI container and DB connection.
///		Derive from it when need DI and DB connection
/// </summary>
// ReSharper disable once InconsistentNaming
public abstract class TestDbBase(ITestOutputHelper _output) : TestBase, IAsyncLifetime
{
	private DataContext DataContext => Container.GetRequiredService<DataContext>();
	
	/// <summary>
	///		Tables that shouldn't be touched on whipping out the DB
	/// </summary>
	private readonly Table[] _tablesToIgnore = [Microsoft.EntityFrameworkCore.Migrations.HistoryRepository.DefaultTableName /* "__EFMigrationsHistory" */];

	/// <summary>
	///     Configure the DI container
	/// </summary>
	protected override void ConfigureIocContainer(IServiceCollection services)
	{
		var sqlConnection = GetSqlConnectionStringFromConfiguration();
		
		services.AddAndConfigureDbContext(sqlConnection, false);
		
		base.ConfigureIocContainer(services);
	}

	/// <summary>
	///		Seed an invoice in the DB
	/// </summary>
	protected Task SeedInvoice(string number, long clientId, DateOnly date, decimal amount)
		=> SeedData (new Invoice
			{
				Number = number, 
				ClientId = clientId,
				Amount = amount,
				Date = date
			});

	/// <summary>
	///		Seed a client in the DB
	/// </summary>
	protected async Task<long> SeedClient(string name = "Test Client 1")
	{
		await SeedData(new Client { Name = name });
		var client = await DataContext.Clients.OrderByDescending(d => d.Id).FirstOrDefaultAsync();
		return client!.Id;
	}

	#region Clean up of DB records -------------------------------------------- 
	
	/// <summary>
	///		Called immediately by xUnit after the test class has been created, before it is used.
	/// </summary>
	public async Task InitializeAsync()
	{
		// Ensures that the database exists and create if not
		await DataContext.Database.EnsureCreatedAsync();

		// Clean up the DB on start, so we can review the state if a test throws an exception
		await WipeOutDbAsync();
	}

	/// <summary>
	///		Called when an object is no longer needed. Called just before <see cref="IDisposable.Dispose"/>
	///		if the class also implements that.
	/// </summary>
	public Task DisposeAsync() => Task.CompletedTask;

	/// <summary>
	///		Wipe out all data in the database
	/// </summary>
	private async Task WipeOutDbAsync()
	{
		Respawner? respawn = null;
		try
		{
			var connectionString = DataContext.Database.GetConnectionString();
			ArgumentNullException.ThrowIfNull(connectionString);
			
			respawn = await Respawner.CreateAsync(connectionString, new RespawnerOptions { TablesToIgnore = _tablesToIgnore });
			await respawn.ResetAsync(connectionString);
		}
		catch (ArgumentNullException ergExc)
		{
			_output.WriteLine(ergExc.Message);
			throw;
		}
		catch(Exception e)
		{
			_output.WriteLine(e.Message +" \n"+ (respawn?.DeleteSql ?? "no delete SQL"));
			throw;
		} 
	}
	#endregion // Clean up of DB records --------------------------------------

	protected override void Dispose(bool disposing)
	{
		if (disposing)
			// dispose all managed resources.
			DataContext.Dispose();
		
		base.Dispose(disposing);
	}

	/// <summary>
	///		Perform <paramref name="dataContextFunc"/> on a new scope of DataContext, to avoid skewed test results due to the EF cache.  
	/// </summary>
	/// <param name="dataContextFunc"> The function to perform on the newly created DAtaContext scope </param>
	protected async Task ScopedDataContextExecAsync(Func<DataContext, Task> dataContextFunc)
	{
		using var assertionScope = Container.CreateScope();
		await using var dataContext = assertionScope.ServiceProvider.GetRequiredService<DataContext>();
		{
			// Disable change tracking
			dataContext.ChangeTracker.AutoDetectChangesEnabled = false;
			await dataContextFunc(dataContext);
		}
	}

	private Task SeedData(params object[] entities)
		=> ScopedDataContextExecAsync(
			async context =>
			{
				await context.AddRangeAsync(entities);
				await context.SaveChangesAsync();
			});
	
	private string GetSqlConnectionStringFromConfiguration()
	{
		var settings = new ConfigurationBuilder()
			.AddJsonFile("testsettings.json", optional: true)
			.AddUserSecrets<TestDbBase>()
			.AddEnvironmentVariables()
			.Build();
		return settings.GetSection("ConnectionString").Value!;
	}
}