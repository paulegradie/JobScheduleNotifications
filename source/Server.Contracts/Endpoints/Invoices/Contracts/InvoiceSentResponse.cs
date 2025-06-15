using Server.Contracts.Dtos;

namespace Server.Contracts.Endpoints.Invoices.Contracts;

public sealed record InvoiceSentResponse(InvoiceDto Invoice);
public sealed record InvoiceSentLegacyResponse(InvoiceLegacyDto InvoiceDto);
