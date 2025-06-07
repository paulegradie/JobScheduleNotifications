using Api.ValueTypes;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts;
using Server.Contracts.Endpoints.Invoices.Contracts;

namespace Mobile.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly IServerClient _serverClient;

    public InvoiceRepository(IServerClient serverClient)
    {
        _serverClient = serverClient;
    }

    public async Task<bool> SendInvoiceAsync(string outputPath, CustomerId currentCustomerId, ScheduledJobDefinitionId currentJobDefinitionId, JobOccurrenceId currentJobOccurrenceId)
    {
        if (!File.Exists(outputPath))
            return false;

        await using var stream = File.OpenRead(outputPath);

        var request = new SendInvoiceRequest(
            CustomerId: currentCustomerId,
            ScheduledJobDefinitionId: currentJobDefinitionId,
            JobOccurrenceId: currentJobOccurrenceId,
            PdfStream: stream,
            FileName: Path.GetFileName(outputPath)
        );

        var result = await _serverClient.Invoices.SendInvoice(request, CancellationToken.None);
        return result.IsSuccess;
    }
}