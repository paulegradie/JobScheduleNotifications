namespace Api.Business.Entities;

public class BusinessOwnerDomainModelBase : DomainModelBase
{
    public BusinessOwnerDomainModelBase()
    {
    }

    public int Id { get; set; }
    public string Email { get; init; }
    public string Password { get; init; }
    public string BusinessName { get; init; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string BusinessDescription { get; set; }

    public bool IsActive { get; set; } = true;
}