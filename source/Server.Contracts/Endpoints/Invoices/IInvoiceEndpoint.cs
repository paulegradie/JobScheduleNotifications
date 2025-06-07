using Server.Contracts.Endpoints.Invoices.Contracts;

namespace Server.Contracts.Endpoints.Invoices;

public interface IInvoiceEndpoint
{
    Task<OperationResult<InvoiceSentResponse>> SendInvoice(SendInvoiceRequest request, CancellationToken cancellationToken);
}