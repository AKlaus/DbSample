using AK.DbSample.Domain.Services.Invoice;
using AK.DbSample.Domain.Services.Invoice.DTOs;
using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace AK.DbSample.Domain.Tests.Services.Invoice;

public class InvoiceCreateUpdateDeleteTests : TestDbBase
{
	private IInvoiceCommandService InvoiceCommandService => Container.GetRequiredService<IInvoiceCommandService>();

	public InvoiceCreateUpdateDeleteTests(ITestOutputHelper output) : base(output) {}
	
	[Fact]
	public async Task Create_Invoice_Works()
	{
		// GIVEN a client
		var clientId = await SeedClient("Name");
		
		// WHEN create a new invoice for the client
		var (invoiceId, result) = await InvoiceCommandService.Create(new CreateInvoiceRequest("INV-01", DateOnly.Parse("2020-07-07"), clientId, 20));
		
		// THEN client appears in the DB
		Assert.True(result.IsSuccess);
		await ScopedDataContextExecAsync(
			async context =>
			{
				var invoice = await context.Invoices.FindAsync(invoiceId);
				Assert.NotNull(invoice);
				Assert.Equal("INV-01", invoice.Number);
				Assert.Equal(DateOnly.Parse("2020-07-07"), invoice.Date);
				Assert.Equal(clientId, invoice.ClientId);
				Assert.Equal(20, invoice.Amount);
			});
	}
	
	[Fact]
	public async Task Update_Invoice_Works()
	{
		// GIVEN a client & an invoice
		var clientId = await SeedClient("Name");
		await SeedInvoice("INV-01", clientId, DateOnly.Parse("2020-07-07"), 20);
		
		// WHEN update amount of the invoice
		var result = await InvoiceCommandService.Update("INV-01", new UpdateInvoiceRequest(DateOnly.Parse("2020-07-07"), 30));
		
		// THEN the amount is updated
		Assert.True(result.IsSuccess);
		await ScopedDataContextExecAsync(
			async context =>
			{
				var invoice = await context.Invoices.FindAsync("INV-01");
				Assert.NotNull(invoice);
				Assert.Equal(30, invoice.Amount);
			});
	}
	
	[Fact]
	public async Task Delete_Invoice_Works()
	{
		// GIVEN a client & an invoice
		var clientId = await SeedClient("Name");
		await SeedInvoice("INV-01", clientId, DateOnly.Parse("2020-07-07"), 20);
		
		// WHEN delete the invoice
		var result = await InvoiceCommandService.Delete("INV-01");
		
		// THEN the invoice cease to exist
		Assert.True(result.IsSuccess);
		await ScopedDataContextExecAsync(
			async context =>
			{
				var client = await context.Invoices.FindAsync("INV-01");
				Assert.Null(client);
			});
	}
}