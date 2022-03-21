namespace AK.DbSample.Api.Configuration;

internal static partial class ServiceCollectionExtensions
{
	public static void AddAndConfigureSwagger(this IServiceCollection services, IWebHostEnvironment env)
	{
		if (env.IsProduction())
			// No Swagger for PROD
			return;

		services.AddEndpointsApiExplorer();
		services.AddSwaggerGen(c => c.UseDateOnlyTimeOnlyStringConverters());
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

