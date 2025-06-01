using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Endpoints.JobPhotos.Contracts;

namespace Api.Controllers;

[Authorize]
[ApiController]
public class PhotosController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public PhotosController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpPost(UploadJobPhotoRequest.Route)]
    public async Task<ActionResult<JobPhotoUploadResponse>> UploadPhoto(
        [FromRoute] string customerId,
        [FromRoute] string jobDefinitionId,
        [FromRoute] string jobOccurenceId,
        [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        var savePath = Path.Combine(_env.ContentRootPath, "UploadedPhotos", file.FileName);
        Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

        await using var stream = new FileStream(savePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Ok(new JobPhotoUploadResponse(new JobPhotoUploadDto(true, savePath, file.Length)));
    }
}