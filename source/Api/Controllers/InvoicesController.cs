using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Dtos;
using Server.Contracts.Endpoints.Invoices.Contracts;

namespace Api.Controllers;

[Authorize]
[ApiController]
public class InvoicesController : ControllerBase
{
    private readonly IWebHostEnvironment _env;

    public InvoicesController(IWebHostEnvironment env)
    {
        _env = env;
    }

    [HttpPost(SendInvoiceRequest.Route)]
    public async Task<ActionResult<InvoiceSentResponse>> UploadInvoice(
        [FromRoute] string customerId,
        [FromRoute] string jobDefinitionId,
        [FromRoute] string jobOccurenceId,
        [FromForm(Name = "file")] IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided.");

        var savePath = Path.Combine(_env.ContentRootPath, "UploadedInvoices", file.FileName);
        Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

        await using var stream = new FileStream(savePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return Ok(new InvoiceSentResponse(new InvoiceSentDto(true, savePath, file.Length)));
    }
}