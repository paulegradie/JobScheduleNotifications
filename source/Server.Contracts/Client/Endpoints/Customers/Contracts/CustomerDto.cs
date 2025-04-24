namespace Server.Contracts.Client.Endpoints.Customers.Contracts;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Notes { get; init; }
    public DateTime CreatedAt { get; set; }
}