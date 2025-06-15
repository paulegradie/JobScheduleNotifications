using Api.Business.Entities.Base;
using Api.ValueTypes;
using Server.Contracts.Dtos;

namespace Api.Business.Entities;

public class CustomerDomainModel : DomainModelBase<CustomerDto>
{
    public CustomerId CustomerId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public OrganizationId OrganizationId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public override CustomerDto ToDto()
    {
        return new CustomerDto
        {
            Id = CustomerId,
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            PhoneNumber = PhoneNumber,
            Notes = Notes
        };
    }

    public override void FromDto(CustomerDto dto)
    {
        CustomerId = dto.Id;
        FirstName = dto.FirstName;
        LastName = dto.LastName;
        Email = dto.Email;
        PhoneNumber = dto.PhoneNumber;
        Notes = dto.Notes;
    }
}
