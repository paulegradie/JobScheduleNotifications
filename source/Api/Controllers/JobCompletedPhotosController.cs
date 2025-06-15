using Api.Infrastructure.Data;
using Api.Infrastructure.DbTables.Jobs;
using Api.ValueTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Contracts.Endpoints.JobPhotos.Contracts;

namespace Api.Controllers;

[Authorize]
[ApiController]
public class JobCompletedPhotosController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly AppDbContext _dbContext;

    public JobCompletedPhotosController(IWebHostEnvironment env, AppDbContext dbContext)
    {
        _env = env;
        _dbContext = dbContext;
    }

    [HttpPost(UploadJobCompletedPhotoRequest.Route)]
    public async Task<ActionResult<JobCompletedPhotoUploadResponse>> UploadPhoto(
        [FromRoute] string customerId,
        [FromRoute] string jobDefinitionId,
        [FromRoute] string jobOccurenceId,
        [FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file provided.");

        var saveDir = Path.Combine(_env.ContentRootPath, "UploadedPhotos");
        Directory.CreateDirectory(saveDir);
        var filePath = Path.Combine(saveDir, $"{Guid.NewGuid()}_{file.FileName}");

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var photo = new JobCompletedPhoto
        {
            JobCompletedPhotoId = new JobCompletedPhotoId(Guid.NewGuid()),
            CustomerId = new CustomerId(Guid.Parse(customerId)),
            JobOccurrenceId = new JobOccurrenceId(Guid.Parse(jobOccurenceId)),
            LocalFilePath = filePath
        };

        // Save to DB
        _dbContext.JobCompletedPhotos.Add(photo);
        await _dbContext.SaveChangesAsync();

        return Ok(new JobCompletedPhotoUploadResponse(new JobCompletedPhotoUploadDto(true, filePath, file.Length)));
    }

    [HttpDelete(DeleteJobCompletedPhotoRequest.Route)]
    public async Task<IActionResult> DeletePhoto(string customerId, string jobDefinitionId, string jobOccurenceId, string jobCompletedPhotoId)
    {
        var id = new JobCompletedPhotoId(Guid.Parse(jobCompletedPhotoId));
        var photo = await _dbContext.JobCompletedPhotos.FindAsync(id);

        if (photo == null) return NotFound();

        // DO NOT DELETE PHOTO FROM SYSTEM - JUST DELETE THE DB RECORD
        // if (System.IO.File.Exists(photo.FilePath)) 
        //     System.IO.File.Delete(photo.FilePath);

        _dbContext.JobCompletedPhotos.Remove(photo);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpGet(ListJobCompletedPhotosRequest.Route)]
    public async Task<IActionResult> GetList(
        [FromRoute] string customerId,
        [FromRoute] string jobDefinitionId,
        [FromRoute] string jobOccurenceId)
    {
        var photosRecords = await _dbContext.JobCompletedPhotos.ToListAsync();
        var photos = photosRecords
            .Where(p => p.JobOccurrenceId.ToString() == jobOccurenceId)
            .Select(p => new JobCompletedPhotoDetails(p.LocalFilePath))
            .ToList();

        var response = new JobCompletedPhotoListResponse(new JobCompletedPhotoListDto(photos));
        return Ok(response);
    }
}