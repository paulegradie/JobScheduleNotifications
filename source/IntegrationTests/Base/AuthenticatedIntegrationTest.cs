using Server.Contracts.Client.Endpoints.Auth;

namespace IntegrationTests.Base;

public class AuthenticatedIntegrationTest : IntegrationTest
{
    protected readonly RegisterNewAdminRequest TestUser = new RegisterNewAdminRequest
    {
        Email = "testowner@gmail.com",
        Password = "TestPassword123!",
        BusinessName = "Paul",
        FirstName = "Paul",
        LastName = "Gradie",
        PhoneNumber = "860753044",
        BusinessDescription = "Demo Business"
    };

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        var registered = await Client.Auth.RegisterAsync(TestUser, CancellationToken);
        if (!registered.IsSuccess)
        {
            throw new InvalidOperationException($"User registration failed during test setup - {registered.ErrorMessage}");
        }

        var token = await Client.Auth.LoginAsync(new SignInRequest(TestUser.Email, TestUser.Password), CancellationToken);

        if (token?.Value is null || string.IsNullOrEmpty(token.Value.AccessToken))
            throw new InvalidOperationException("Login failed — no token returned");

        Client.Http.DefaultRequestHeaders.Authorization = new("Bearer", token.Value.AccessToken);
    }
}