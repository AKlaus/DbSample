using Microsoft.Extensions.Options;

namespace AK.DbSample.Api.Configuration;

internal static partial class ServiceCollectionExtensions
{
	/// <summary>
	///		Register Global Settings
	/// </summary>
	public static AppSettings AddAndConfigureAppSettings(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddOptions<AppSettings>()
			.ValidateDataAnnotations()
			.ValidateOnStart();;

		services.Configure<AppSettings>(configuration, c => c.BindNonPublicProperties = true);
		services.AddSingleton(r => r.GetRequiredService<IOptions<AppSettings>>().Value);
		
		// Makes IServiceProvider available in the container.
		// Note that this step may resolve in duplicates of registered objects, so apply responsibly
		var resolver = services.BuildServiceProvider();

		return resolver.GetRequiredService<AppSettings>();
	}
}