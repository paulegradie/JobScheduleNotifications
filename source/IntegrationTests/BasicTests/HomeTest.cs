using IntegrationTests.Base;
using Server.Contracts.Client.Endpoints.Home;
using Shouldly;

namespace IntegrationTests.BasicTests;

public class HomeTest : AuthenticatedIntegrationTest
{
    [Fact]
    public async Task PingHome()
    {
        var response = await Client.Home.PingHomeAsync(new HomeRequest(), CancellationToken);
        response.IsSuccess.ShouldBeTrue();
        response.Value.ShouldNotBeNull();
        response.Value.Hello.ShouldBe("Hello from the API!");
    }
}