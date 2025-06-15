using Api.Business.Entities.Base;
using Api.Business.Services;
using Api.ValueTypes;
using Server.Contracts.Dtos;

namespace Api.Business.Entities;

public class InvoiceDomainModel : DomainModelBase<InvoiceDto>
{
    public InvoiceId InvoiceId { get; set; }
    public JobOccurrenceId JobOccurrenceId { get; set; }
    public CustomerId CustomerId { get; set; }
    
    // File storage information
    public string FileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public FileStorageLocation StorageLocation { get; set; }
    public long FileSizeBytes { get; set; }
    
    // Invoice metadata
    public DateTime CreatedDate { get; set; }
    public DateTime? EmailSentDate { get; set; }
    public string? EmailSentTo { get; set; }

    public static InvoiceDomainModel Create(
        JobOccurrenceId jobOccurrenceId,
        CustomerId customerId,
        string fileName,
        string filePath,
        FileStorageLocation storageLocation,
        long fileSizeBytes)
    {
        return new InvoiceDomainModel
        {
            InvoiceId = InvoiceId.New(),
            JobOccurrenceId = jobOccurrenceId,
            CustomerId = customerId,
            FileName = fileName,
            FilePath = filePath,
            StorageLocation = storageLocation,
            FileSizeBytes = fileSizeBytes,
            CreatedDate = DateTime.UtcNow
        };
    }

    public void MarkEmailSent(string emailAddress)
    {
        EmailSentDate = DateTime.UtcNow;
        EmailSentTo = emailAddress;
    }

    public override InvoiceDto ToDto()
    {
        return new InvoiceDto(
            InvoiceId: InvoiceId,
            JobOccurrenceId: JobOccurrenceId,
            CustomerId: CustomerId,
            FileName: FileName,
            FilePath: FilePath,
            StorageLocation: StorageLocation.ToString(),
            FileSizeBytes: FileSizeBytes,
            CreatedDate: CreatedDate,
            EmailSentDate: EmailSentDate,
            EmailSentTo: EmailSentTo
        );
    }

    public override void FromDto(InvoiceDto dto)
    {
        InvoiceId = dto.InvoiceId;
        JobOccurrenceId = dto.JobOccurrenceId;
        CustomerId = dto.CustomerId;
        FileName = dto.FileName;
        FilePath = dto.FilePath;
        StorageLocation = Enum.Parse<FileStorageLocation>(dto.StorageLocation);
        FileSizeBytes = dto.FileSizeBytes;
        CreatedDate = dto.CreatedDate;
        EmailSentDate = dto.EmailSentDate;
        EmailSentTo = dto.EmailSentTo;
    }
}
