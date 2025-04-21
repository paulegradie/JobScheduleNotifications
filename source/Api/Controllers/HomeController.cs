using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Client.Endpoints.Home;

namespace Api.Controllers;

[Authorize]
public class HomeController : BaseApiController
{
    [HttpGet(HomeRequest.Route)]
    public async Task<ActionResult<HomeResponse>> Get()
    {
        await Task.Yield();
        return new HomeResponse("Hello from the API!");
    }
}