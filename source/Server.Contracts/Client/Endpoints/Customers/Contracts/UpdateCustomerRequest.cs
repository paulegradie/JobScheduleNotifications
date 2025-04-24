using Server.Contracts.Client.Request;

namespace Server.Contracts.Client.Endpoints.Customers.Contracts;

public sealed record UpdateCustomerRequest : RequestBase
{
    public const string Route = "api/customers/update/{id}";

    public override ApiRoute GetApiRoute()
    {
        var route = base.GetApiRoute();
        route.AddRouteParam("id", Id.ToString());
        return route;
    }

    private UpdateCustomerRequest(
        Guid id,
        string? firstName,
        string? lastName,
        string? phoneNumber,
        string? email,
        string? notes)
        : base(Route)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        Notes = notes;
    }

    public Guid Id { get; set; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? PhoneNumber { get; init; }
    public string? Email { get; init; }
    public string? Notes { get; init; }

    public static Builder CreateBuilder(Guid id) => new(id);

    public sealed class Builder(Guid Id)
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
            // Optional validation
            if (string.IsNullOrWhiteSpace(_email))
                throw new InvalidOperationException("Email is required when updating customer.");

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