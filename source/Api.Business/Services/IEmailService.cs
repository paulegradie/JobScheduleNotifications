namespace Api.Business.Services;

public interface IEmailService
{
    /// <summary>
    /// Sends an email with an invoice attachment
    /// </summary>
    /// <param name="toEmail">Recipient email address</param>
    /// <param name="customerName">Name of the customer</param>
    /// <param name="jobTitle">Title of the job</param>
    /// <param name="invoiceFileUrl">URL or path to the invoice file</param>
    /// <param name="invoiceFileName">Name of the invoice file</param>
    Task<bool> SendInvoiceEmailAsync(
        string toEmail, 
        string customerName, 
        string jobTitle, 
        string invoiceFileUrl, 
        string invoiceFileName);
}
