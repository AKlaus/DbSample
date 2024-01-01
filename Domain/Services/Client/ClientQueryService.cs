using AK.DbSample.Database;
using AK.DbSample.Domain.Services.Client.DTOs;

using DomainResults.Common;

using Microsoft.EntityFrameworkCore;

namespace AK.DbSample.Domain.Services.Client;

public interface IClientQueryService
{
	Task<(GetClientByIdResponse, IDomainResult)> GetById(long clientId);
	Task<GetClientListResponse[]> GetList(GetClientListRequest filter);
}

public class ClientQueryService(DataContext dataContext) : BaseService(dataContext), IClientQueryService
{
	public async Task<(GetClientByIdResponse, IDomainResult)> GetById(long clientId)
	{
		var client = await DataContext.Clients
				.Include(c => c.Invoices)
				.SingleOrDefaultAsync(c => c.Id == clientId);
		if (client == null)
			return IDomainResult.NotFound<GetClientByIdResponse>("Client not found");

		var lastInvoiceDate = client.Invoices.OrderByDescending(i=>i.Date).Take(1).SingleOrDefault()?.Date;
		
		return IDomainResult.Success(new GetClientByIdResponse(client.Name, lastInvoiceDate));
	}
	
	public async Task<GetClientListResponse[]> GetList(GetClientListRequest filter)
	{
		var query = from c in DataContext.Clients.Include(c => c.Invoices)
				select new GetClientListResponse(
						c.Id, 
						c.Name,
						c.Invoices
							.OrderByDescending(i => i.Date)
							.Take(1)
							.SingleOrDefault().Date
					);

		if (!string.IsNullOrWhiteSpace(filter.Name))
			query = query.Where(c => c.Name.StartsWith(filter.Name));

		return await query.ToArrayAsync();
	}
}