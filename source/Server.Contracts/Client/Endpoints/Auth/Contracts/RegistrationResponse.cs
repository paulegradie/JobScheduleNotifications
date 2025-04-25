namespace Server.Contracts.Client.Endpoints.Auth.Contracts;

public sealed record RegistrationResponse(bool Registered, string? Message);