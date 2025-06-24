using Api.ValueTypes;

namespace Server.Contracts.Dtos;

public class OrganizationSettingsDto
{
    public OrganizationId OrganizationId { get; set; }
    
    // Business Information
    public string BusinessName { get; set; } = string.Empty;
    public string BusinessDescription { get; set; } = string.Empty;
    public string BusinessIdNumber { get; set; } = string.Empty;
    
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
    
    public string FormattedBankDetails => 
        !string.IsNullOrEmpty(BankBsb) && !string.IsNullOrEmpty(BankAccountNumber)
            ? $"BSB: {BankBsb}, Account: {BankAccountNumber}"
            : string.Empty;
            
    public string FullAddress
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(StreetAddress)) parts.Add(StreetAddress);
            if (!string.IsNullOrEmpty(City)) parts.Add(City);
            if (!string.IsNullOrEmpty(State)) parts.Add(State);
            if (!string.IsNullOrEmpty(PostalCode)) parts.Add(PostalCode);
            if (!string.IsNullOrEmpty(Country)) parts.Add(Country);
            return string.Join(", ", parts);
        }
    }
}
