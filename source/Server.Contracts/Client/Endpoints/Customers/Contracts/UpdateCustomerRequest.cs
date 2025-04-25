using Api.ValueTypes;
using Server.Contracts.Common.Request;

namespace Server.Contracts.Client.Endpoints.Customers.Contracts;

public sealed record UpdateCustomerRequest : RequestBase
{
    public const string Route = $"api/customers/update/{IdSegmentParam}";

    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam(IdSegmentParam, Id.ToString());
        return route;
    }

    public UpdateCustomerRequest(CustomerId id) : base(Route)
    {
        Id = id;
    }

    private UpdateCustomerRequest(
        CustomerId id,
        string? firstName = null,
        string? lastName = null,
        string? phoneNumber = null,
        string? email = null,
        string? notes = null)
        : base(Route)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        Notes = notes;
    }

    public CustomerId Id { get; set; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Notes { get; init; }

    public static Builder CreateBuilder(CustomerId id) => new(id);

    public sealed class Builder(CustomerId Id)
    {
        private string? _firstName;
        private string? _lastName;
        private string? _phoneNumber;
        private string? _email;
        private string? _notes;


        public Builder WithFirstName(string firstName)
        {
            _firstName = firstName;
            return this;
        }

        public Builder WithLastName(string lastName)
        {
            _lastName = lastName;
            return this;
        }

        public Builder WithPhoneNumber(string phoneNumber)
        {
            _phoneNumber = phoneNumber;
            return this;
        }

        public Builder WithEmail(string email)
        {
            _email = email;
            return this;
        }

        public Builder WithNotes(string notes)
        {
            _notes = notes;
            return this;
        }

        public UpdateCustomerRequest Build()
        {
            return new UpdateCustomerRequest(
                id: Id,
                firstName: _firstName,
                lastName: _lastName,
                phoneNumber: _phoneNumber,
                email: _email,
                notes: _notes);
        }
    }
}