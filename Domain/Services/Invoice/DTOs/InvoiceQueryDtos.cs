namespace AK.DbSample.Domain.Services.Invoice.DTOs;

public record GetInvoiceListRequest(long? ClientId);
public record GetInvoiceListResponse (string Number, DateOnly Date, decimal Amount, ClientReference Client);

public record GetInvoiceByNumberResponse(DateOnly Date, decimal Amount, ClientReference Client);

public record ClientReference(long Id, string Name);
