using AK.DbSample.Database;
using AK.DbSample.Domain.Services.Invoice.DTOs;

using DomainResults.Common;

using Microsoft.EntityFrameworkCore;

namespace AK.DbSample.Domain.Services.Invoice;

public interface IInvoiceCommandService
{
	Task<(string, IDomainResult)> Create(CreateInvoiceRequest dto);
	Task<IDomainResult> Update(string number, UpdateInvoiceRequest dto);
	Task<IDomainResult> Delete(string number);
}

public class InvoiceCommandService(DataContext dataContext) : BaseService(dataContext), IInvoiceCommandService
{
	public async Task<(string, IDomainResult)> Create(CreateInvoiceRequest dto)
	{
		var numberCheckResult = await UniqueNumberCheck(dto.Number, true);
		if (!numberCheckResult.IsSuccess)
			return (string.Empty, numberCheckResult);

		var clientCheckResult = await ClientExistsCheck(dto.ClientId);
		if (!clientCheckResult.IsSuccess)
			return (string.Empty, clientCheckResult);

		var invoice = new Database.Entities.Invoice
			{
				Number = dto.Number, 
				ClientId = dto.ClientId,
				Amount = dto.Amount,
				Date = dto.Date
			};
		DataContext.Invoices.Add(invoice);
		await DataContext.SaveChangesAsync();

		return IDomainResult.Success(invoice.Number);
	}

	public async Task<IDomainResult> Update(string number, UpdateInvoiceRequest dto)
	{
		var invoice = await DataContext.Invoices.FindAsync(number);
		if (invoice == null)
			return IDomainResult.NotFound("Invoice not found");

		var numberCheckResult = await UniqueNumberCheck(number, false);
		if (!numberCheckResult.IsSuccess)
			return numberCheckResult;

		invoice.Date = dto.Date;
		invoice.Amount = dto.Amount;
		await DataContext.SaveChangesAsync();

		return IDomainResult.Success();
	}

	public async Task<IDomainResult> Delete(string number)
	{
		var invoiceForDeleting = await DataContext.Invoices.FindAsync(number);
		if (invoiceForDeleting == null)
			return IDomainResult.NotFound("Invoice not found");
		
		DataContext.Invoices.Remove(invoiceForDeleting);
		await DataContext.SaveChangesAsync();

		return IDomainResult.Success();
	}

	private async Task<IDomainResult> UniqueNumberCheck(string newNumber, bool creatingNew)
	{
		if (string.IsNullOrWhiteSpace(newNumber))
			return IDomainResult.Failed("Number can't be empty");
		
		if (await DataContext.Invoices.AsNoTracking().Where(c => c.Number == newNumber).AnyAsync()
		    && creatingNew)
			return IDomainResult.Failed($"Invoice number '{newNumber}' already exists");
		
		return IDomainResult.Success();
	}

	private async Task<IDomainResult> ClientExistsCheck(long clientId)
	{
		return !await DataContext.Clients.AsNoTracking().Where(c => c.Id == clientId).AnyAsync()
			? IDomainResult.NotFound("Client not found")
			: IDomainResult.Success();
	}
}