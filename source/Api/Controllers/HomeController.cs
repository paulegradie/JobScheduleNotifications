using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Client.Endpoints.Home;

namespace JobScheduleNotifications.Api.Controllers;

public class HomeController : BaseApiController
{
    // [HttpGet(HomeRequest.ActionRoute)]
    // public Task<HomeResponse> Get() => Task.FromResult(new HomeResponse("Hello from the API!"));
    
    [HttpGet(HomeRequest.Route)]  // Use the same route constant
    public async Task<ActionResult<HomeResponse>> Get([FromQuery] HomeRequest request)
    {
        return new HomeResponse("Hello");
    }

}