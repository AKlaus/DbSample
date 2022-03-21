namespace AK.DbSample.Domain.Services.Invoice.DTOs;

public record CreateInvoiceRequest(string Number, DateOnly Date, long ClientId, decimal Amount);

public record UpdateInvoiceRequest(DateOnly Date, decimal Amount);