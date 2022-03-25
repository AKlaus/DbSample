using AK.DbSample.Domain.Services.Invoice;
using AK.DbSample.Domain.Services.Invoice.DTOs;

using DomainResults.Mvc;

using Microsoft.AspNetCore.Mvc;

namespace AK.DbSample.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class InvoicesController : ControllerBase
{
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<GetInvoiceListResponse[]> GetList([FromServices] IInvoiceQueryService service, [FromQuery] GetInvoiceListRequest request)
		=> await service.GetList(request);
	
	[HttpGet("{number}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<GetInvoiceByNumberResponse>> GetByNumber([FromServices] IInvoiceQueryService service, string number)
		=> await service.GetByNumber(number).ToActionResultOfT();

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<string>> Create([FromServices] IInvoiceCommandService service, CreateInvoiceRequest request)
		=> await service.Create(request).ToActionResultOfT();

	[HttpPut("{number}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Update([FromServices] IInvoiceCommandService service, string number, UpdateInvoiceRequest request)
		=> await service.Update(number, request).ToActionResult();

	[HttpDelete("{number}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Delete([FromServices] IInvoiceCommandService service, string number)
		=> await service.Delete(number).ToActionResult();
}