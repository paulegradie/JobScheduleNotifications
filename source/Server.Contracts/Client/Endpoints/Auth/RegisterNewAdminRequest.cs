using Server.Contracts.Client.Request;


public record RegisterNewAdminRequest() : RequestBase(Route)
{
        public const string Route = "/api/user/register";
        
        public string Email { get; init; }
        public string Password { get; init; }
        public string BusinessName { get; init; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessAddress { get; set; }
        public string BusinessDescription { get; set; }
}