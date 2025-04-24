namespace Server.Contracts.Client.Endpoints.Auth;

public sealed record RegistrationResponse(bool Registered, string? Message);