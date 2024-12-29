using AK.DbSample.Domain.Services.Invoice;
using AK.DbSample.Domain.Services.Invoice.DTOs;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace AK.DbSample.Domain.Tests.Services.Invoice;

public class InvoiceQueryTests : TestDbBase
{
	private IInvoiceCommandService InvoiceCommandService => Container.GetRequiredService<IInvoiceCommandService>();
	private IInvoiceQueryService InvoiceQueryService => Container.GetRequiredService<IInvoiceQueryService>();

	public InvoiceQueryTests(ITestOutputHelper output) : base(output){}
	
	[Fact]
	public async Task Get_Invoice_By_Number_Works()
	{
		// GIVEN a client & an invoice
		var clientId = await SeedClient("Name");
		await SeedInvoice("INV-01", clientId, DateOnly.Parse("2020-07-07"), 20);
		
		// WHEN get invoice by number
		var (invoice, result) = await InvoiceQueryService.GetByNumber("INV-01");
		
		// THEN invoice gets resolved
		Assert.True(result.IsSuccess);
		Assert.Equal(20, invoice.Amount);
	}
	
	[Fact]
	public async Task Get_Invoice_List_Works()
	{
		// GIVEN a client & 2 invoices
		var clientId = await SeedClient("Name");
		await SeedInvoice("INV-01", clientId, DateOnly.Parse("2020-07-07"), 10);
		await SeedInvoice("INV-02", clientId, DateOnly.Parse("2020-08-07"), 20);
		
		// WHEN get a list of invoices
		var invoices = await InvoiceQueryService.GetList(new GetInvoiceListRequest());
		
		// THEN get 2 invoices
		Assert.Equal(2, invoices.Length);
		var orderedList = invoices.OrderBy(c => c.Number).ToArray();
		Assert.Equal(new[] {"INV-01", "INV-02"}, orderedList.Select(c => c.Number));
		Assert.Equal(new[] {DateOnly.Parse("2020-07-07"), DateOnly.Parse("2020-08-07")}, orderedList.Select(c => c.Date));
		Assert.Equal(new[] {10m, 20m}, orderedList.Select(c => c.Amount));
	}
	
	[Fact]
	public async Task Get_Invoice_List_Filtered_By_Client_Works()
	{
		// GIVEN a client 1 with an invoice
		var client1Id = await SeedClient("Homer Simpson");
		await SeedInvoice("INV-01", client1Id, DateOnly.Parse("2020-07-07"), 10);
		// and a client 2 with an invoice
		var client2Id = await SeedClient("Marge Simpson");
		await SeedInvoice("INV-02", client2Id, DateOnly.Parse("2020-08-07"), 20);
		
		// WHEN get a list of invoices for client 1
		var invoices = await InvoiceQueryService.GetList(new GetInvoiceListRequest(client1Id));
		
		// THEN get 1 invoice
		Assert.Single(invoices);
		var invoice = invoices.Single();
		Assert.Equal("INV-01", invoice.Number);
		Assert.Equal(DateOnly.Parse("2020-07-07"), invoice.Date);
		Assert.Equal(10, invoice.Amount);
	}
}