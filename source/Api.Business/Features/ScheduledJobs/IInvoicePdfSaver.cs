namespace Api.Controllers;

public interface IInvoicePdfSaver
{
    Task<bool> SaveInvoiceAsync(IFormFile file);
}