using AK.DbSample.Database;
using AK.DbSample.Domain.Services.Client.DTOs;

using DomainResults.Common;

using Microsoft.EntityFrameworkCore;

namespace AK.DbSample.Domain.Services.Client;

public interface IClientCommandService
{
	Task<(long, IDomainResult)> Create(CreateUpdateClientRequest dto);
	Task<IDomainResult> Update(long id, CreateUpdateClientRequest dto);
	Task<IDomainResult> Delete(long id);
}

public class ClientCommandService(DataContext dataContext) : BaseService(dataContext), IClientCommandService
{
	public async Task<(long, IDomainResult)> Create(CreateUpdateClientRequest dto)
	{
		var nameCheckResult = await UniqueNameCheck(null, dto.Name);
		if (!nameCheckResult.IsSuccess)
			return (default, nameCheckResult);

		var client = new Database.Entities.Client { Name = dto.Name };
		DataContext.Clients.Add(client);
		await DataContext.SaveChangesAsync();

		return IDomainResult.Success(client.Id);
	}

	public async Task<IDomainResult> Update(long id, CreateUpdateClientRequest dto)
	{
		var client = await DataContext.Clients.FindAsync(id);
		if (client == null)
			return IDomainResult.NotFound("Invalid ID");
		
		var nameCheckResult = await UniqueNameCheck(null, dto.Name);
		if (!nameCheckResult.IsSuccess)
			return nameCheckResult;

		client.Name = dto.Name;
		await DataContext.SaveChangesAsync();

		return IDomainResult.Success();
	}

	public async Task<IDomainResult> Delete(long id)
	{
		var clientForDeleting = await DataContext.Clients.FindAsync(id);
		if (clientForDeleting == null)
			return IDomainResult.NotFound("Client not found");
		
		DataContext.Clients.Remove(clientForDeleting);
		await DataContext.SaveChangesAsync();

		return IDomainResult.Success();
	}

	private async Task<IDomainResult> UniqueNameCheck(long? id, string name)
	{
		if (string.IsNullOrWhiteSpace(name))
			return IDomainResult.Failed("Name can't be empty");
		
		var clientsQuery = DataContext.Clients.AsNoTracking().Where(c => c.Name == name);
		if (id.HasValue)
			clientsQuery = clientsQuery.Where(c => c.Id != id);
		
		if (await clientsQuery.AnyAsync())
			return IDomainResult.Failed($"Client with name '{name}' already exists");
		
		return IDomainResult.Success();
	}
}