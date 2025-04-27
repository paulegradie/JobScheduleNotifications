using Api.Controllers.Base;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Endpoints.Home.Contracts;

namespace Api.Controllers;

public class HomeController : BaseApiController
{
    [HttpGet(HomeRequest.Route)]
    public async Task<ActionResult<HomeResponse>> Get()
    {
        await Task.Yield();
        return new HomeResponse("Hello from the API!");
    }
}