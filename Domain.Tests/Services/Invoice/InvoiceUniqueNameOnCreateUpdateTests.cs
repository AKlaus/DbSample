using AK.DbSample.Domain.Services.Invoice;
using AK.DbSample.Domain.Services.Invoice.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Xunit;
using Xunit.Abstractions;

namespace AK.DbSample.Domain.Tests.Services.Invoice;

public class InvoiceUniqueNameOnCreateUpdateTests : TestDbBase
{
	private IInvoiceCommandService InvoiceCommandService => Container.GetRequiredService<IInvoiceCommandService>();

	public InvoiceUniqueNameOnCreateUpdateTests(ITestOutputHelper output) : base(output){}
	
	[Fact]
	public async Task Create_Invoice_With_Non_Unique_Number_Fails()
	{
		// GIVEN a client & an invoice
		var clientId = await SeedClient("Name");
		await SeedInvoice("INV-01", clientId, DateOnly.Parse("2020-07-07"), 20);
		
		// WHEN create a new invoice with the same number
		var (_, result) = await InvoiceCommandService.Create(new CreateInvoiceRequest("INV-01", DateOnly.Parse("2020-08-01"), clientId, 10));
		
		// THEN operation fails
		Assert.False(result.IsSuccess);
		await ScopedDataContextExecAsync(
			async context =>
			{
				var invoiceCount = await context.Invoices.CountAsync();
				Assert.Equal(1, invoiceCount);
			});
	}
}