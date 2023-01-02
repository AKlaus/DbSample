using AK.DbSample.Domain.Services.Client;
using AK.DbSample.Domain.Services.Client.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace AK.DbSample.Domain.Tests.Services.Client;

public class ClientUniqueNameOnCreateUpdateTests : TestDbBase
{
	private IClientCommandService ClientCommandService => Container.GetRequiredService<IClientCommandService>();

	public ClientUniqueNameOnCreateUpdateTests(ITestOutputHelper output) : base(output){}
	
	[Fact]
	public async Task Create_Client_With_Non_Unique_Name_Fails()
	{
		// GIVEN a DB with a client
		await SeedClient("Name");
		
		// WHEN create a new client with the same name
		var (_, result) = await ClientCommandService.Create(new CreateUpdateClientRequest("Name"));
		
		// THEN operation fails
		Assert.False(result.IsSuccess);
		var clientCount = await DataContext.Clients.CountAsync();
		Assert.Equal(1, clientCount);
	}
	
	[Fact]
	public async Task Update_Client_With_Existing_Name_Fails()
	{
		// GIVEN a DB with a client
		await SeedClient("Name1");
		var clientId = await SeedClient("Name2");
		
		// WHEN rename client 'Name2' to 'Name1'
		var result = await ClientCommandService.Update(clientId, new CreateUpdateClientRequest("Name1"));
		
		// THEN operation fails
		Assert.False(result.IsSuccess);
	}
}