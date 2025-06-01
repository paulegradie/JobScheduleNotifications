using Server.Contracts.Endpoints.Reminders.Contracts;

namespace Server.Contracts.Endpoints.Reminders;

public interface IInvoiceEndpoint
{
    Task<OperationResult<InvoiceSentResponse>> SendInvoice(SendInvoiceRequest request, CancellationToken cancellationToken);
}