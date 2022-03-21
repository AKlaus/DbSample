using AK.DbSample.Api.Configuration;
using AK.DbSample.Domain.Configuration;

namespace AK.DbSample.Api;

public class Startup
{
	private readonly IConfiguration _configuration;
	private readonly IWebHostEnvironment _hostingEnvironment;

	public Startup(IConfiguration config, IWebHostEnvironment hostEnvironment)
	{
		_configuration = config;
		_hostingEnvironment = hostEnvironment;
	}
	
	// This method gets called by the runtime. Use this method to add services to the container.
	public void ConfigureServices(IServiceCollection services)
	{
		var settings = services.AddAndConfigureAppSettings(_configuration);
		
		services.AddAndConfigureDomainServices((settings.ConnectionString, true));

		services
			.AddControllers(options => options.UseDateOnlyTimeOnlyStringConverters())
			.AddJsonOptions(options => options.UseDateOnlyTimeOnlyStringConverters());

		services.AddAndConfigureSwagger(_hostingEnvironment);
	}

	public void Configure(IApplicationBuilder app)
	{
		if (_hostingEnvironment.IsProduction())
		{
			// Adds a header to access via HTTPS only (default HSTS value is 30 days)
			app.UseHsts();
			app.UseHttpsRedirection();
		}
		else
			app.AddAppSwaggerUi(_hostingEnvironment);
		
		// Matches request to an endpoint
		app.UseRouting();
		// Executes the matched endpoint
		app.UseEndpoints(endpoints => endpoints.MapControllers());
	}
}