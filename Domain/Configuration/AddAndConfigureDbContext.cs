using AK.DbSample.Database;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AK.DbSample.Domain.Configuration;

public static partial class ServiceCollectionExtensions
{
	/// <summary>
	///		Register the main databases
	/// </summary>
	public static void AddAndConfigureDbContext(this IServiceCollection services, string? connectionString, bool registerMigrationsAssembly)
	{
		services.AddDbContext<DataContext>((_, options) =>
			{
				connectionString = connectionString?.Trim();

				// Need to use a dummy connection to run 'ef migration' command in the build pipeline.
				// It seems to be the recommended solution - https://github.com/dotnet/efcore/issues/1470#issuecomment-163353265
				// Moved dummy string into user secrets and added to load through
				if (string.IsNullOrWhiteSpace(connectionString))
					connectionString =  "Server=localhost,1433;InitialCatalog=AssistedCoreLogging";

				// Enabling retries for resiliency, see: https://docs.microsoft.com/en-us/ef/core/miscellaneous/connection-resiliency
				options.UseSqlServer(connectionString,
					sqlOptions =>
					{
						sqlOptions.EnableRetryOnFailure();
						if (registerMigrationsAssembly)
							sqlOptions.MigrationsAssembly(typeof(DataContext).Assembly.FullName);
					});
			});
	}
}
