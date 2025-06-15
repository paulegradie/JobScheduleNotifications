using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public record InvoiceDto(
    InvoiceId InvoiceId,
    JobOccurrenceId JobOccurrenceId,
    CustomerId CustomerId,
    string FileName,
    string FilePath,
    string StorageLocation,
    long FileSizeBytes,
    DateTime CreatedDate,
    DateTime? EmailSentDate,
    string? EmailSentTo
);

// Keep the old DTO for backward compatibility
public record InvoiceLegacyDto(bool Success, string FileUri, long FileSize);