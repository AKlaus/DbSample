using AK.DbSample.Domain.Services.Client;
using AK.DbSample.Domain.Services.Client.DTOs;

using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace AK.DbSample.Domain.Tests.Services.Client;

public class ClientCreateUpdateDeleteTests : TestDbBase
{
	private IClientCommandService ClientCommandService => Container.GetRequiredService<IClientCommandService>();

	public ClientCreateUpdateDeleteTests(ITestOutputHelper output) : base(output) {}
	
	[Fact]
	public async Task Create_Client_Works()
	{
		// GIVEN empty DB
		
		// WHEN create a new client
		var (clientId, result) = await ClientCommandService.Create(new CreateUpdateClientRequest("Test"));
		
		// THEN client appears in the DB
		Assert.True(result.IsSuccess);
		var client = await DataContext.Clients.FindAsync(clientId);
		Assert.NotNull(client);
		Assert.Equal("Test", client!.Name);
	}
	
	[Fact]
	public async Task Update_Client_Works()
	{
		// GIVEN a DB with a client
		var clientId = await SeedClient("Name");
		
		// WHEN update name of the client
		var result = await ClientCommandService.Update(clientId, new CreateUpdateClientRequest("XYZ"));
		
		// THEN the name is updated
		Assert.True(result.IsSuccess);
		var client = await DataContext.Clients.FindAsync(clientId);
		Assert.NotNull(client);
		Assert.Equal("XYZ", client!.Name);
	}
	
	[Fact]
	public async Task Delete_Client_Works()
	{
		// GIVEN a DB with a client
		var clientId = await SeedClient();
		
		// WHEN delete the client
		var result = await ClientCommandService.Delete(clientId);
		
		// THEN the client cease to exist
		Assert.True(result.IsSuccess);
		var client = await DataContext.Clients.FindAsync(clientId);
		Assert.Null(client);
	}
}