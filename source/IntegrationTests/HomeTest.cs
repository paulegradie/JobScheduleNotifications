using IntegrationTests.Base;
using Server.Contracts.Client.Endpoints.Home;
using Shouldly;

public class HomeTest : AuthenticatedIntegrationTest
{
    [Fact]
    public async Task PingHome()
    {
        var response = await Client.Home.PingHome(new HomeRequest(), CancellationToken);
        response.Hello.ShouldBe("Hello from the API!");
    }   
}