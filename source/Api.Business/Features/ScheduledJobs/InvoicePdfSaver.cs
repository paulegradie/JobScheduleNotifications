using Api.Controllers;

namespace Api.Business.Features.ScheduledJobs;

public class InvoicePdfSaver : IInvoicePdfSaver
{
    private readonly AppDbContext _appDbContext;
    private readonly IWebHostEnvironment _env;

    public InvoicePdfSaver(AppDbContext appDbContext, IWebHostEnvironment env, IInvoiceRepository invoiceRepository)
    {
        _appDbContext = appDbContext;
        _env = env;
    }

    public async Task<bool> SaveInvoiceAsync(IFormFile file, string fileName, )
    {
        if (file is null || file.Length == 0) return false;

        try
        {
            var savePath = Path.Combine(_env.ContentRootPath, "UploadedInvoices", file.FileName);
            Directory.CreateDirectory(Path.GetDirectoryName(savePath)!);

            await using var stream = new FileStream(savePath, FileMode.Create);
            await file.CopyToAsync(stream);
            
            
            
            return true;
        }
        catch (IOException ex)
        {
            return false;
        }
    }
}