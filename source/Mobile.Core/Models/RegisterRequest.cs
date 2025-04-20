namespace Mobile.Core.Models;

public record RegisterRequest(
    string Email,
    string Password,
    string BusinessName,
    string FirstName,
    string LastName,
    string PhoneNumber,
    string BusinessAddress,
    string BusinessDescription);
    