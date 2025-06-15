namespace Server.Contracts.Dtos;

public record InvoiceDto(bool Success, string FileUri, long FileSize);