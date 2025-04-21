using Server.Contracts.Client.Endpoints.Auth;

namespace IntegrationTests.Base;

public class AuthenticatedIntegrationTest : IntegrationTest
{
    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // 🔹 Seed a test user (or use your existing user creation method)
        var auth = Client.Auth;

        var testUser = new RegisterNewAdminRequest
        {
            Email = "test@example.com",
            Password = "TestPassword123!",
            BusinessName = "Paul",
            FirstName = "Paul",
            LastName = "G",
            PhoneNumber = "860753044",
            BusinessAddress = "1 place drive, CT",
            BusinessDescription = "Demo Business"
        };

        var registered = await auth.RegisterAsync(testUser);
        if (!registered)
            throw new InvalidOperationException("User registration failed during test setup");

        var token = await auth.LoginAsync(new SignInRequest("test@example.com", "TestPassword123!"));

        if (token is null || string.IsNullOrEmpty(token.AccessToken))
            throw new InvalidOperationException("Login failed — no token returned");

        Client.Http.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token.AccessToken);
    }
}