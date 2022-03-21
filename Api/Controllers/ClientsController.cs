using AK.DbSample.Domain.Services.Client;
using AK.DbSample.Domain.Services.Client.DTOs;
using DomainResults.Mvc;
using Microsoft.AspNetCore.Mvc;

namespace AK.DbSample.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ClientsController : ControllerBase
{
	[HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task<GetClientListResponse[]> GetList([FromServices] IClientQueryService service, GetClientListRequest request)
		=> await service.GetList(request);
	
	[HttpGet("{clientId}")]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<ActionResult<GetClientByIdResponse>> GetById([FromServices] IClientQueryService service, long clientId)
		=> await service.GetById(clientId).ToActionResultOfT();

	[HttpPost]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<ActionResult<long>> Create([FromServices] IClientCommandService service, CreateUpdateClientRequest request)
		=> await service.Create(request).ToActionResultOfT();

	[HttpPut("{clientId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> Update([FromServices] IClientCommandService service, long clientId, CreateUpdateClientRequest request)
		=> await service.Update(clientId, request).ToActionResult();

	[HttpDelete("{clientId}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> Delete([FromServices] IClientCommandService service, long clientId)
		=> await service.Delete(clientId).ToActionResult();
}