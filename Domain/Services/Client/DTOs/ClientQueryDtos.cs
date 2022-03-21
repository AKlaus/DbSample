namespace AK.DbSample.Domain.Services.Client.DTOs;

public record GetClientListRequest(string? Name);
public record GetClientListResponse (long Id, string Name, DateOnly? LastInvoice);

public record GetClientByIdResponse(string Name, DateOnly? LastInvoiceDate);
