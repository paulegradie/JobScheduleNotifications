using Server.Contracts.Client.Endpoints.Auth;

namespace IntegrationTests.Base;

public class AuthenticatedIntegrationTest : IntegrationTest
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // 🔹 Seed a test user (or use your existing user creation method)
        var testUser = new RegisterNewAdminRequest
        {
            Email = "test@gmail.com",
            Password = "TestPassword123!",
            BusinessName = "Paul",
            FirstName = "Paul",
            LastName = "G",
            PhoneNumber = "860753044",
            BusinessDescription = "Demo Business"
        };
        var registered = await Client.Auth.RegisterAsync(testUser, CancellationToken);
        if (!registered.IsSuccess)
        {
            throw new InvalidOperationException($"User registration failed during test setup - {registered.ErrorMessage}");
        }

        var token = await Client.Auth.LoginAsync(new SignInRequest("test@gmail.com", "TestPassword123!"), CancellationToken);

        if (token is null || string.IsNullOrEmpty(token.Value.AccessToken))
            throw new InvalidOperationException("Login failed — no token returned");

        Client.Http.DefaultRequestHeaders.Authorization = new("Bearer", token.Value.AccessToken);
    }
}