using IntegrationTests.Base;
using Server.Contracts.Client.Endpoints.Auth;
using Server.Contracts.Client.Endpoints.Auth.Contracts;
using Shouldly;

namespace IntegrationTests.BasicTests;

public class AuthenticationTests : IntegrationTest
{
    private readonly RegisterNewAdminRequest _newOwner = new RegisterNewAdminRequest
    {
        Email = "testowner@gmail.com",
        Password = "TestPassword123!",
        BusinessName = "Paul",
        FirstName = "Paul",
        LastName = "Gradie",
        PhoneNumber = "860753044",
        BusinessDescription = "Demo Business"
    };

    [Fact]
    public async Task RegistrationWorks()
    {
        var registered = await Client.Auth.RegisterAsync(_newOwner, CancellationToken);
        registered.IsSuccess.ShouldBeTrue();
        registered.Value.ShouldNotBeNull();
        registered.ErrorMessage.ShouldBeNullOrEmpty();
    }

    [Fact]
    public async Task RegistrationAndLoginWorks()
    {
        await Client.Auth.RegisterAsync(_newOwner, CancellationToken);
        var token = await Client.Auth.LoginAsync(new SignInRequest(_newOwner.Email, _newOwner.Password), CancellationToken);

        token.Value.ShouldNotBeNull();
        token.Value.AccessToken.ShouldNotBeNullOrEmpty();
    }
}