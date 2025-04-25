using IntegrationTests.Base;
using Server.Contracts.Client.Endpoints.Home;
using Server.Contracts.Client.Endpoints.Home.Contracts;
using Shouldly;

namespace IntegrationTests.BasicTests;

public class HomeTest : IntegrationTest
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