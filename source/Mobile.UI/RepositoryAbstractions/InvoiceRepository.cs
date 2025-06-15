﻿using Api.ValueTypes;
using Mobile.UI.Pages.Customers.ScheduledJobs.JobOccurrences;
using Server.Contracts.Endpoints;
using Server.Contracts.Endpoints.Invoices.Contracts;

namespace Mobile.UI.RepositoryAbstractions;

public interface IInvoiceRepository
{
    Task<OperationResult<InvoiceSentResponse>> SendInvoiceAsync(string outputPath, CustomerId currentCustomerId, ScheduledJobDefinitionId currentJobDefinitionId, JobOccurrenceId currentJobOccurrenceId);
    Task<OperationResult<InvoiceSavedResponse>> SaveInvoiceAsync(string outputPath, IEnumerable<InvoiceItem> invoiceItems, CustomerId customerId, ScheduledJobDefinitionId jobDefinitionId, JobOccurrenceId jobOccurrenceId);
    Task<OperationResult<InvoiceSentResponse>> SendInvoiceAsync(InvoiceId invoiceId, CustomerId customerId, ScheduledJobDefinitionId jobDefinitionId, JobOccurrenceId jobOccurrenceId);
}