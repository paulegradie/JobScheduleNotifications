using Api.ValueTypes;

namespace Mobile.UI.RepositoryAbstractions;

public interface IInvoiceRepository
{
    Task<bool> SendInvoiceAsync(string outputPath, CustomerId currentCustomerId, ScheduledJobDefinitionId currentJobDefinitionId, JobOccurrenceId currentJobOccurrenceId);
}