﻿using IntegrationTests.Base;
using Server.Contracts.Endpoints.Home.Contracts;
using Shouldly;

namespace IntegrationTests.ApiTests;

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