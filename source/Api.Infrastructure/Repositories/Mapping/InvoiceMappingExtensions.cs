using Api.Business.Entities;
using Api.Business.Services;
using Api.Infrastructure.DbTables.Jobs;

namespace Api.Infrastructure.Repositories.Mapping;

public static class InvoiceMappingExtensions
{
    public static InvoiceDomainModel ToDomainModel(this Invoice entity)
    {
        return new InvoiceDomainModel
        {
            InvoiceId = entity.InvoiceId,
            JobOccurrenceId = entity.JobOccurrenceId,
            CustomerId = entity.CustomerId,
            FileName = entity.FileName,
            FilePath = entity.FilePath,
            StorageLocation = entity.StorageLocation,
            FileSizeBytes = entity.FileSizeBytes,
            CreatedDate = entity.CreatedDate,
            EmailSentDate = entity.EmailSentDate,
            EmailSentTo = entity.EmailSentTo
        };
    }

    public static Invoice ToDbEntity(this InvoiceDomainModel domain)
    {
        return new Invoice
        {
            InvoiceId = domain.InvoiceId,
            JobOccurrenceId = domain.JobOccurrenceId,
            CustomerId = domain.CustomerId,
            FileName = domain.FileName,
            FilePath = domain.FilePath,
            StorageLocation = domain.StorageLocation,
            FileSizeBytes = domain.FileSizeBytes,
            CreatedDate = domain.CreatedDate,
            EmailSentDate = domain.EmailSentDate,
            EmailSentTo = domain.EmailSentTo
        };
    }

    public static void UpdateFromDomainModel(this Invoice entity, InvoiceDomainModel domain)
    {
        entity.FileName = domain.FileName;
        entity.FilePath = domain.FilePath;
        entity.StorageLocation = domain.StorageLocation;
        entity.FileSizeBytes = domain.FileSizeBytes;
        entity.EmailSentDate = domain.EmailSentDate;
        entity.EmailSentTo = domain.EmailSentTo;
        // Note: We don't update InvoiceId, JobOccurrenceId, CustomerId, or CreatedDate
    }
}
