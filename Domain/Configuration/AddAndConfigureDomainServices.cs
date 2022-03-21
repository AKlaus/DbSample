using AK.DbSample.Domain.Helpers;
using AK.DbSample.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace AK.DbSample.Domain.Configuration;

public static partial class ServiceCollectionExtensions
{
	public static void AddAndConfigureDomainServices(this IServiceCollection services, (string? connectionString, bool registerMigrationsAssembly)? configureDatabase = null)
	{
		if (configureDatabase.HasValue)
			services.AddAndConfigureDbContext(configureDatabase.Value.connectionString, configureDatabase.Value.registerMigrationsAssembly);

		var types = typeof(BaseService).Assembly
			.GetTypes()
			.Where(t =>
						!t.IsAbstract
						&& t.IsAssignableTo<BaseService>()		// All services
				).ToList();

		services.RegisterAsImplementedInterfaces(types, ServiceLifetime.Scoped);
	}
}