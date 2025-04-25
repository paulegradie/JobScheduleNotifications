using Server.Contracts.Common.Request;
using Server.Contracts.Dtos;

namespace Server.Contracts.Client.Endpoints.Customers.Contracts;

public sealed record CreateCustomerResponse(CustomerDto Customer);

public sealed record CreateCustomerRequest : RequestBase
{
    public const string Route = "api/customers/create";


    public CreateCustomerRequest(
        string firstName,
        string lastName,
        string phoneNumber,
        string email,
        string notes) : base(Route)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        Notes = notes;
    }

    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string PhoneNumber { get; init; }
    public string Email { get; init; }
    public string Notes { get; set; }
}