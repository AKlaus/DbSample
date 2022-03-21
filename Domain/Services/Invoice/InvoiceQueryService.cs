using AK.DbSample.Database;
using AK.DbSample.Domain.Services.Invoice.DTOs;
using DomainResults.Common;
using Microsoft.EntityFrameworkCore;

namespace AK.DbSample.Domain.Services.Invoice;

public interface IInvoiceQueryService
{
	Task<(GetInvoiceByNumberResponse, IDomainResult)> GetByNumber(string number);
	Task<GetInvoiceListResponse[]> GetList(GetInvoiceListRequest filter);
}

public class InvoiceQueryService: BaseService, IInvoiceQueryService
{
	public InvoiceQueryService(DataContext dataContext) : base(dataContext) {}

	public async Task<(GetInvoiceByNumberResponse, IDomainResult)> GetByNumber(string number)
	{
		var invoice = await DataContext.Invoices
				.Include(i => i.Client)
				.SingleOrDefaultAsync(c => c.Number == number);
		if (invoice == null)
			return IDomainResult.NotFound<GetInvoiceByNumberResponse>("Invoice not found");

		return IDomainResult.Success(
			new GetInvoiceByNumberResponse(
				invoice.Date, 
				invoice.Amount, 
				new ClientReference(invoice.Client.Id, invoice.Client.Name)
			)
		);
	}
	
	public async Task<GetInvoiceListResponse[]> GetList(GetInvoiceListRequest filter)
	{
		var query = DataContext.Invoices
				.Include(i => i.Client)
				.Select(i => i);
		if (filter.ClientId != null)
			query = query.Where(i => i.ClientId == filter.ClientId);

		var ret = (await query.ToArrayAsync())
			.Select(i =>
				new GetInvoiceListResponse(i.Number, i.Date, i.Amount, new ClientReference(i.Client.Id, i.Client.Name))
			).ToArray();

		return ret;
	}
}