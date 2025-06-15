﻿﻿﻿﻿using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Business.Services;
using Api.ValueTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Dtos;
using Server.Contracts.Endpoints.Invoices.Contracts;

namespace Api.Controllers;

[Authorize]
[ApiController]
public class InvoicesController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IEmailService _emailService;
    private readonly ICustomerRepository _customerRepository;

    public InvoicesController(
        IFileStorageService fileStorageService,
        IInvoiceRepository invoiceRepository,
        IEmailService emailService,
        ICustomerRepository customerRepository)
    {
        _fileStorageService = fileStorageService;
        _invoiceRepository = invoiceRepository;
        _emailService = emailService;
        _customerRepository = customerRepository;
    }

    [HttpPost(SaveInvoiceRequest.Route)]
    public async Task<ActionResult<InvoiceSavedResponse>> SaveInvoice(
        [FromRoute] string customerId,
        [FromRoute] string jobDefinitionId,
        [FromRoute] string jobOccurenceId,
        [FromForm(Name = "file")] IFormFile file)
    {
        if (file is null || file.Length == 0)
            return BadRequest("No file provided.");

        try
        {
            // Parse IDs
            var customerIdParsed = CustomerId.Parse(customerId);
            var jobOccurrenceIdParsed = JobOccurrenceId.Parse(jobOccurenceId);

            // Save file to storage
            await using var stream = file.OpenReadStream();
            var filePath = await _fileStorageService.SaveFileAsync(stream, file.FileName, file.ContentType);

            // Create and save invoice domain model
            var invoice = InvoiceDomainModel.Create(
                jobOccurrenceIdParsed,
                customerIdParsed,
                file.FileName,
                filePath,
                _fileStorageService.StorageLocation,
                file.Length);

            var savedInvoice = await _invoiceRepository.SaveInvoiceAsync(invoice);

            return Ok(new InvoiceSavedResponse(savedInvoice.ToDto()));
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error saving invoice: {ex.Message}");
        }
    }

    [HttpPost(SendInvoiceRequest.Route)]
    public async Task<ActionResult<InvoiceSentResponse>> SendInvoice(
        [FromRoute] string customerId,
        [FromRoute] string jobDefinitionId,
        [FromRoute] string jobOccurenceId,
        [FromRoute] string invoiceId)
    {
        try
        {
            // Parse IDs
            var customerIdParsed = CustomerId.Parse(customerId);
            var invoiceIdParsed = InvoiceId.Parse(invoiceId);

            // Get the invoice
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceIdParsed);
            if (invoice == null)
                return NotFound("Invoice not found.");

            // Get customer details for email
            var customer = await _customerRepository.GetByIdAsync(customerIdParsed);
            if (customer == null)
                return NotFound("Customer not found.");

            if (string.IsNullOrEmpty(customer.Email))
                return BadRequest("Customer email address is not set.");

            // Send email
            var emailSent = await _emailService.SendInvoiceEmailAsync(
                customer.Email,
                $"{customer.FirstName} {customer.LastName}",
                "Job Invoice", // You might want to get the actual job title
                invoice.FilePath,
                invoice.FileName);

            if (emailSent)
            {
                // Mark invoice as sent
                invoice.MarkEmailSent(customer.Email);
                await _invoiceRepository.UpdateAsync(invoice);

                return Ok(new InvoiceSentResponse(invoice.ToDto()));
            }
            else
            {
                return StatusCode(500, "Failed to send email.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error sending invoice: {ex.Message}");
        }
    }
}