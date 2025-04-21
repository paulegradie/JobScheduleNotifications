namespace Server.Contracts.Client.Endpoints.Auth;

public record LoginRequest(string Email, string Password);