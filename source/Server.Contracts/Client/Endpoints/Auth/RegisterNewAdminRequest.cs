using System.ComponentModel.DataAnnotations;
using Server.Contracts.Client.Request;

namespace Server.Contracts.Client.Endpoints.Auth
{
    public record RegisterNewAdminRequest() : RequestBase(Route)
    {
        public const string Route = "/api/user/register";
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public required string Email { get; init; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character")]
        public required string Password { get; init; }

        [Required(ErrorMessage = "Business name is required")]
        [MinLength(2, ErrorMessage = "Business name must be at least 2 characters long")]
        [MaxLength(100, ErrorMessage = "Business name cannot exceed 100 characters")]
        public required string BusinessName { get; init; }

        [Required(ErrorMessage = "First name is required")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters long")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public required string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [MinLength(2, ErrorMessage = "Last name must be at least 2 characters long")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public required string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Business address is required")]
        [MinLength(5, ErrorMessage = "Business address must be at least 5 characters long")]
        [MaxLength(200, ErrorMessage = "Business address cannot exceed 200 characters")]
        public required string BusinessAddress { get; set; }

        [MaxLength(500, ErrorMessage = "Business description cannot exceed 500 characters")]
        public required string BusinessDescription { get; set; }
    }
}