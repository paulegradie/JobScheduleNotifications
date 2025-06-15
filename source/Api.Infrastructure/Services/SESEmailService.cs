﻿﻿﻿using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Api.Business.Services;
using Api.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Net.Mime;

namespace Api.Infrastructure.Services;

public class SESEmailService : IEmailService
{
    private readonly IAmazonSimpleEmailService _sesClient;
    private readonly EmailConfiguration _config;
    private readonly IFileStorageService _fileStorageService;

    public SESEmailService(
        IAmazonSimpleEmailService sesClient,
        IOptions<EmailConfiguration> config,
        IFileStorageService fileStorageService)
    {
        _sesClient = sesClient;
        _config = config.Value;
        _fileStorageService = fileStorageService;
    }

    public async Task<bool> SendInvoiceEmailAsync(
        string toEmail, 
        string customerName, 
        string jobTitle, 
        string invoiceFileUrl, 
        string invoiceFileName)
    {
        try
        {
            var subject = $"Invoice for {jobTitle}";
            var bodyText = $@"
Dear {customerName},

Please find attached your invoice for the completed job: {jobTitle}.

Thank you for your business!

Best regards,
Your Service Team
";

            var bodyHtml = $@"
<html>
<body>
    <h2>Invoice for {jobTitle}</h2>
    <p>Dear {customerName},</p>
    <p>Please find attached your invoice for the completed job: <strong>{jobTitle}</strong>.</p>
    <p>Thank you for your business!</p>
    <p>Best regards,<br/>Your Service Team</p>
</body>
</html>";

            // For SES, we'll send a simple email with a link to download the invoice
            // Note: SES doesn't support attachments directly in the simple send API
            // For attachments, you'd need to use SES with raw email or use a different approach
            
            var downloadUrl = await _fileStorageService.GetFileUrlAsync(invoiceFileUrl);
            
            var bodyWithLink = $@"
<html>
<body>
    <h2>Invoice for {jobTitle}</h2>
    <p>Dear {customerName},</p>
    <p>Your invoice for the completed job: <strong>{jobTitle}</strong> is ready.</p>
    <p><a href=""{downloadUrl}"" style=""background-color: #4CAF50; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px;"">Download Invoice</a></p>
    <p>Thank you for your business!</p>
    <p>Best regards,<br/>Your Service Team</p>
</body>
</html>";

            var request = new SendEmailRequest
            {
                Source = _config.FromEmail,
                Destination = new Destination
                {
                    ToAddresses = new List<string> { toEmail }
                },
                Message = new Message
                {
                    Subject = new Content(subject),
                    Body = new Body
                    {
                        Text = new Content(bodyText),
                        Html = new Content(bodyWithLink)
                    }
                }
            };

            var response = await _sesClient.SendEmailAsync(request);
            return !string.IsNullOrEmpty(response.MessageId);
        }
        catch (Exception)
        {
            return false;
        }
    }
}


