using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public class CustomerDto
{
    public CustomerId Id { get; set; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; set; }

    public string FullName => $"{FirstName} {LastName}";
}