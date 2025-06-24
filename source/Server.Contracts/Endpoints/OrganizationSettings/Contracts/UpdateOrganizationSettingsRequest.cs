using Server.Contracts.Common.Request;
using Server.Contracts.Dtos;

namespace Server.Contracts.Endpoints.OrganizationSettings.Contracts;

public sealed record UpdateOrganizationSettingsRequest : RequestBase
{
    public const string Route = "api/organization/settings";

    public UpdateOrganizationSettingsRequest(
        string businessName,
        string businessDescription,
        string businessIdNumber,
        string email,
        string phoneNumber,
        string streetAddress,
        string city,
        string state,
        string postalCode,
        string country,
        string bankName,
        string bankBsb,
        string bankAccountNumber,
        string bankAccountName) : base(Route)
    {
        BusinessName = businessName;
        BusinessDescription = businessDescription;
        BusinessIdNumber = businessIdNumber;
        Email = email;
        PhoneNumber = phoneNumber;
        StreetAddress = streetAddress;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        BankName = bankName;
        BankBsb = bankBsb;
        BankAccountNumber = bankAccountNumber;
        BankAccountName = bankAccountName;
    }

    // Business Information
    public string BusinessName { get; init; }
    public string BusinessDescription { get; init; }
    public string BusinessIdNumber { get; init; }
    
    // Contact Information
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
    
    // Address Information
    public string StreetAddress { get; init; }
    public string City { get; init; }
    public string State { get; init; }
    public string PostalCode { get; init; }
    public string Country { get; init; }
    
    // Banking Information
    public string BankName { get; init; }
    public string BankBsb { get; init; }
    public string BankAccountNumber { get; init; }
    public string BankAccountName { get; init; }
}

public sealed record UpdateOrganizationSettingsResponse(OrganizationSettingsDto Settings);
