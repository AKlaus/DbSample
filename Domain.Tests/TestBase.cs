using AK.DbSample.Domain.Configuration;

using Microsoft.Extensions.DependencyInjection;

namespace AK.DbSample.Domain.Tests;

/// <summary>
///		Base test class with a DI container. 
///		Derive from it when need DI but not DB connection
/// </summary>
public abstract class TestBase : IDisposable
{
	protected ServiceProvider Container { get; }
	
	protected TestBase()
	{
		var services = new ServiceCollection();
		// ReSharper disable once VirtualMemberCallInConstructor
		ConfigureIocContainer(services);
		Container = services.BuildServiceProvider();
	}

	/// <summary>
	///     Configure the DI container
	/// </summary>
	protected virtual void ConfigureIocContainer(IServiceCollection services)
	{
		services.AddAndConfigureDomainServices();
	}

	/// <summary>
	///		Returns a string with random content (8 char long)
	/// </summary>
	protected static string GetRandomString()
	{
		const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		var stringChars = new char[8];
		var random = new Random();

		for (var i = 0; i < stringChars.Length; i++)
		{
			stringChars[i] = chars[random.Next(chars.Length)];
		}

		return new string(stringChars);
	}

	#region IDisposable implementation ----------------------------------------
	
	// Followed by the guideline from https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose

	/// <summary>
	///		Flag to detect redundant calls
	/// </summary>
	private bool _disposed;
	
	~TestBase() => Dispose(false);

	/// <summary>
	///		Public implementation of Dispose pattern callable by consumers
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}		

	/// <summary>
	///		Dispose objects
	/// </summary>
	/// <param name="disposing">
	///		true - Managed and unmanaged resources can be disposed.
	///			   (has been called directly or indirectly by a user's code)
	///		false - Only unmanaged resources can be disposed.
	///			   (has been called by the runtime from inside the finalizer and you should not reference other objects)
	/// </param>
	protected virtual void Dispose(bool disposing)
	{
		if (_disposed) return;

		if (disposing)
			// dispose all managed resources.
			Container.Dispose();
		
		// Call the appropriate methods to clean up unmanaged resources here.
		// If 'disposing' is false, only the following code is executed.
		
		_disposed = true;
	}
	#endregion // IDisposable implementation ----------------------------------
}