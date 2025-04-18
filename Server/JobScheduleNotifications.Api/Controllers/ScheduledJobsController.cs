using JobScheduleNotifications.Core.Entities;
using JobScheduleNotifications.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduleNotifications.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduledJobsController : ControllerBase
{
    private readonly IRepository<ScheduledJob> _scheduledJobRepository;
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<BusinessOwner> _businessOwnerRepository;
    private readonly IRepository<JobReminder> _jobReminderRepository;

    public ScheduledJobsController(
        IRepository<ScheduledJob> scheduledJobRepository,
        IRepository<Customer> customerRepository,
        IRepository<BusinessOwner> businessOwnerRepository,
        IRepository<JobReminder> jobReminderRepository)
    {
        _scheduledJobRepository = scheduledJobRepository;
        _customerRepository = customerRepository;
        _businessOwnerRepository = businessOwnerRepository;
        _jobReminderRepository = jobReminderRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduledJob>>> GetAll()
    {
        var jobs = await _scheduledJobRepository.GetAllAsync();
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScheduledJob>> GetById(Guid id)
    {
        var job = await _scheduledJobRepository.GetByIdAsync(id);
        if (job == null)
        {
            return NotFound();
        }
        return Ok(job);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<ScheduledJob>>> GetByCustomer(Guid customerId)
    {
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
        {
            return NotFound("Customer not found");
        }

        var jobs = customer.ScheduledJobs;
        return Ok(jobs);
    }

    [HttpGet("business/{businessOwnerId}")]
    public async Task<ActionResult<IEnumerable<ScheduledJob>>> GetByBusinessOwner(Guid businessOwnerId)
    {
        var businessOwner = await _businessOwnerRepository.GetByIdAsync(businessOwnerId);
        if (businessOwner == null)
        {
            return NotFound("Business owner not found");
        }

        var jobs = businessOwner.ScheduledJobs;
        return Ok(jobs);
    }

    [HttpPost]
    public async Task<ActionResult<ScheduledJob>> Create(ScheduledJob job)
    {
        // Verify customer exists
        var customer = await _customerRepository.GetByIdAsync(job.CustomerId);
        if (customer == null)
        {
            return BadRequest("Customer not found");
        }

        // Verify business owner exists
        var businessOwner = await _businessOwnerRepository.GetByIdAsync(job.BusinessOwnerId);
        if (businessOwner == null)
        {
            return BadRequest("Business owner not found");
        }

        var createdJob = await _scheduledJobRepository.AddAsync(job);
        await _scheduledJobRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = createdJob.Id }, createdJob);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ScheduledJob job)
    {
        if (id != job.Id)
        {
            return BadRequest();
        }

        var existingJob = await _scheduledJobRepository.GetByIdAsync(id);
        if (existingJob == null)
        {
            return NotFound();
        }

        await _scheduledJobRepository.UpdateAsync(job);
        await _scheduledJobRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var job = await _scheduledJobRepository.GetByIdAsync(id);
        if (job == null)
        {
            return NotFound();
        }

        await _scheduledJobRepository.DeleteAsync(job);
        await _scheduledJobRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{jobId}/reminders")]
    public async Task<ActionResult<JobReminder>> AddReminder(Guid jobId, JobReminder reminder)
    {
        var job = await _scheduledJobRepository.GetByIdAsync(jobId);
        if (job == null)
        {
            return NotFound("Scheduled job not found");
        }

        reminder.ScheduledJobId = jobId;
        var createdReminder = await _jobReminderRepository.AddAsync(reminder);
        await _jobReminderRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = createdReminder.Id }, createdReminder);
    }

    [HttpPut("{jobId}/complete")]
    public async Task<IActionResult> MarkAsCompleted(Guid jobId)
    {
        var job = await _scheduledJobRepository.GetByIdAsync(jobId);
        if (job == null)
        {
            return NotFound();
        }

        job.CompletedDate = DateTime.UtcNow;
        await _scheduledJobRepository.UpdateAsync(job);
        await _scheduledJobRepository.SaveChangesAsync();
        return NoContent();
    }
} 