namespace AK.DbSample.Api.Configuration;

internal static partial class ServiceCollectionExtensions
{
	public static IServiceCollection AddAndConfigureSwagger(this IServiceCollection services, IWebHostEnvironment env)
	{
		if (env.IsProduction())
			// No Swagger for PROD
			return services;

		return services
			.AddEndpointsApiExplorer()
			.AddSwaggerGen();
	}
	
	public static void AddAppSwaggerUi(this IApplicationBuilder app, IWebHostEnvironment env)
	{
		if (env.IsProduction())
			// No Swagger for PROD
			return;
		
		app.UseSwagger();
		app.UseSwaggerUI();
	}
}

