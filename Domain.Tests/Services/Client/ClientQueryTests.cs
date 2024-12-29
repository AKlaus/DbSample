using AK.DbSample.Domain.Services.Client;
using AK.DbSample.Domain.Services.Client.DTOs;

using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace AK.DbSample.Domain.Tests.Services.Client;

public class ClientQueryTests : TestDbBase
{
	private IClientQueryService ClientQueryService => Container.GetRequiredService<IClientQueryService>();

	public ClientQueryTests(ITestOutputHelper output) : base(output){}
	
	[Fact]
	public async Task Get_Client_By_Id_Works()
	{
		// GIVEN a DB with a client
		var clientId = await SeedClient("Name");
		
		// WHEN get client by ID
		var (client, result) = await ClientQueryService.GetById(clientId);
		
		// THEN client gets resolved
		Assert.True(result.IsSuccess);
		Assert.Equal("Name", client.Name);
	}
	
	[Fact]
	public async Task Get_Clients_List_Works()
	{
		// GIVEN a DB with 2 clients
		await SeedClient("Name1");
		await SeedClient("Name2");
		
		// WHEN get a list of clients
		var clients = await ClientQueryService.GetList(new GetClientListRequest());
		
		// THEN get 2 clients
		Assert.Equal(2, clients.Length);
		Assert.Equal(new[] {"Name1", "Name2"}, clients.OrderBy(c => c.Name).Select(c=>c.Name));
	}
}