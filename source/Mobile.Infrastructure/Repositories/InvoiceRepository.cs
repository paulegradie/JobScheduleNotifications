using System.Net;
using Api.ValueTypes;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.Invoices.Contracts;

namespace Mobile.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly IServerClient _serverClient;

    public InvoiceRepository(IServerClient serverClient)
    {
        _serverClient = serverClient;
    }

    public async Task<OperationResult<InvoiceSentResponse>> SendInvoiceAsync(
        InvoiceId invoiceId,
        CustomerId currentCustomerId,
        ScheduledJobDefinitionId currentJobDefinitionId,
        JobOccurrenceId currentJobOccurrenceId)
    {
        var request = new SendInvoiceRequest(
            CustomerId: currentCustomerId,
            ScheduledJobDefinitionId: currentJobDefinitionId,
            JobOccurrenceId: currentJobOccurrenceId,
            InvoiceId: invoiceId
        );

        var result = await _serverClient.Invoices.SendInvoice(request, CancellationToken.None);
        return result;
    }

    public async Task<OperationResult<InvoiceSavedResponse>> SaveInvoiceAsync(
        string outputPath,
        IEnumerable<InvoiceItem> invoiceItems,
        CustomerId customerId,
        ScheduledJobDefinitionId jobDefinitionId,
        JobOccurrenceId jobOccurrenceId)
    {
        if (!File.Exists(outputPath))
        {
            return OperationResult<InvoiceSavedResponse>.Failure("Failed to find invoice locally", HttpStatusCode.Ambiguous);
        }

        await using var stream = File.OpenRead(outputPath);
        var request = new SaveInvoiceRequest(
            customerId,
            jobDefinitionId,
            jobOccurrenceId,
            invoiceItems.ToArray(),
            stream,
            Path.GetFileName(outputPath));
        var result = await _serverClient.Invoices.SaveInvoice(request, CancellationToken.None);
        return result;
    }

    public async Task<OperationResult<InvoiceSentResponse>> SendInvoiceAsync(
        string outputPath,
        CustomerId currentCustomerId,
        ScheduledJobDefinitionId currentJobDefinitionId,
        JobOccurrenceId currentJobOccurrenceId)
    {
        // This method is for backward compatibility - it uploads and sends in one step
        // For now, we'll just return a not implemented error
        return OperationResult<InvoiceSentResponse>.Failure("This method is deprecated. Use SendInvoiceAsync with InvoiceId instead.", HttpStatusCode.NotImplemented);
    }
}