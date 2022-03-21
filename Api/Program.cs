using AK.DbSample.Api;

var builder = Host.CreateDefaultBuilder(args)
	.ConfigureWebHostDefaults(webBuilder =>
	{
		webBuilder.ConfigureAppConfiguration((hostingContext, config) =>
		{
			var env = hostingContext.HostingEnvironment;
			if (env.EnvironmentName == "Local")
				config.AddUserSecrets<Startup>();
		});
		webBuilder.UseStartup<Startup>();
	});

var app = builder.Build();
app.Run();