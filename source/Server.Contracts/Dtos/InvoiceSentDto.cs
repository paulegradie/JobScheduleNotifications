namespace Server.Contracts.Dtos;

public record InvoiceSentDto(bool Success, string FilePath, long FileSize);