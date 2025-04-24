using Server.Contracts.Client.Endpoints.Customers.Contracts;

namespace Mobile.Core.Domain;

public class ServiceRecipient
{
    private readonly CustomerDto _customer;

    public ServiceRecipient(CustomerDto customer)
    {
        _customer = customer;
        Id = customer.Id;
        FirstName = customer.FirstName;
        LastName = customer.LastName;
        Email = customer.Email;
        PhoneNumber = customer.PhoneNumber;
        Notes = customer.Notes;
        CreatedAt = customer.CreatedAt;
    }

    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}   
