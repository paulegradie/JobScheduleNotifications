using Api.ValueTypes;

namespace Api.Infrastructure.DbTables.OrganizationModels;

public class OrganizationSettings
{
    public OrganizationId OrganizationId { get; set; }
    public virtual Organization Organization { get; set; } = null!;

    // Business Information
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessDescription { get; set; } = string.Empty;
    public string BusinessIdNumber { get; set; } = string.Empty; // ABN, etc.
    
    // Contact Information
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    
    // Address Information
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    
    // Banking Information
    public string BankName { get; set; } = string.Empty;
    public string BankBsb { get; set; } = string.Empty;
    public string BankAccountNumber { get; set; } = string.Empty;
    public string BankAccountName { get; set; } = string.Empty;
    
    // Audit fields
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    public static OrganizationSettings CreateDefault(OrganizationId organizationId)
    {
        return new OrganizationSettings
        {
            OrganizationId = organizationId,
            CreatedAt = DateTime.UtcNow,
            Country = "Australia" // Default for this app
        };
    }
    
    public void UpdateBusinessInfo(string businessName, string businessDescription, string businessIdNumber)
    {
        BusinessName = businessName;
        BusinessDescription = businessDescription;
        BusinessIdNumber = businessIdNumber;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateContactInfo(string email, string phoneNumber)
    {
        Email = email;
        PhoneNumber = phoneNumber;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateAddress(string streetAddress, string city, string state, string postalCode, string country)
    {
        StreetAddress = streetAddress;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void UpdateBankingInfo(string bankName, string bankBsb, string bankAccountNumber, string bankAccountName)
    {
        BankName = bankName;
        BankBsb = bankBsb;
        BankAccountNumber = bankAccountNumber;
        BankAccountName = bankAccountName;
        UpdatedAt = DateTime.UtcNow;
    }
}
