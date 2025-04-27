using Api.ValueTypes;

namespace Api.Infrastructure.Auth;

public record RegistrationResult(string Email, IdentityUserId IdentityUserId);