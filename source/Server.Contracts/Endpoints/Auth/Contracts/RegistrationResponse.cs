namespace Server.Contracts.Endpoints.Auth.Contracts;

public sealed record RegistrationResponse(bool Registered, string? Message);