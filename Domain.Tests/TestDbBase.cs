using AK.DbSample.Database;
using AK.DbSample.Database.Entities;
using AK.DbSample.Domain.Configuration;

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
public abstract class TestDbBase : TestBase, IAsyncLifetime
{
	protected DataContext DataContext => Container.GetRequiredService<DataContext>();
	
	private readonly Checkpoint _checkPoint = new()
		{
			TablesToIgnore = new Table[] { "__EFMigrationHistory" }
		};
	
	protected readonly ITestOutputHelper Output;
	
	private string _sqlConnection;

	protected TestDbBase(ITestOutputHelper output)
	{
		Output = output;
	}
	
	/// <summary>
	///     Configure the DI container
	/// </summary>
	protected override void ConfigureIocContainer(IServiceCollection services)
	{
		_sqlConnection = GetSqlConnectionStringFromConfiguration();
		services.AddAndConfigureDbContext(_sqlConnection, false);
		base.ConfigureIocContainer(services);
	}

	protected async Task SeedData(params object[] entities)
	{
		await DataContext.AddRangeAsync(entities);
		await DataContext.SaveChangesAsync();
	}

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
	protected async Task WipeOutDbAsync()
	{
		try
		{
			await _checkPoint.Reset(_sqlConnection);
		}
		catch(Exception e)
		{
			Output.WriteLine(e.Message +" \n"+ (_checkPoint.DeleteSql ?? "no delete SQL"));
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
	
	private string GetSqlConnectionStringFromConfiguration()
	{
		var settings = new ConfigurationBuilder()
			.AddJsonFile("testsettings.json", optional: false)
			.AddEnvironmentVariables()
			.Build();
		return settings.GetSection("ConnectionString").Value;
	}
}