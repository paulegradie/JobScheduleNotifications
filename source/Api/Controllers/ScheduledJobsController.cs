using Api.Business.Interfaces;
using Api.Infrastructure.DbTables;
using Api.Infrastructure.DbTables.OrganizationModels;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduledJobsController : ControllerBase
{
    private readonly ICrudRepository<ScheduledJob> _scheduledJobCrudRepository;
    private readonly ICrudRepository<Customer> _customerCrudRepository;
    private readonly ICrudRepository<JobReminder> _jobReminderCrudRepository;

    public ScheduledJobsController(
        ICrudRepository<ScheduledJob> scheduledJobCrudRepository,
        ICrudRepository<Customer> customerCrudRepository,
        ICrudRepository<JobReminder> jobReminderCrudRepository)
    {
        _scheduledJobCrudRepository = scheduledJobCrudRepository;
        _customerCrudRepository = customerCrudRepository;
        _jobReminderCrudRepository = jobReminderCrudRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ScheduledJob>>> GetAll()
    {
        var jobs = await _scheduledJobCrudRepository.GetAllAsync();
        return Ok(jobs);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ScheduledJob>> GetById(Guid id)
    {
        var job = await _scheduledJobCrudRepository.GetByIdAsync(id);
        if (job == null)
        {
            return NotFound();
        }
        return Ok(job);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IEnumerable<ScheduledJob>>> GetByCustomer(Guid customerId)
    {
        var customer = await _customerCrudRepository.GetByIdAsync(customerId);
        if (customer == null)
        {
            return NotFound("Customer not found");
        }

        var jobs = customer.ScheduledJobs;
        return Ok(jobs);
    }

    [HttpPost]
    public async Task<ActionResult<ScheduledJob>> Create(ScheduledJob job)
    {
        // Verify customer exists
        var customer = await _customerCrudRepository.GetByIdAsync(job.CustomerId);
        if (customer == null)
        {
            return BadRequest("Customer not found");
        }

        var createdJob = await _scheduledJobCrudRepository.AddAsync(job);
        await _scheduledJobCrudRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = createdJob.Id }, createdJob);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ScheduledJob job)
    {
        if (id != job.Id)
        {
            return BadRequest();
        }

        var existingJob = await _scheduledJobCrudRepository.GetByIdAsync(id);
        if (existingJob == null)
        {
            return NotFound();
        }

        await _scheduledJobCrudRepository.UpdateAsync(job);
        await _scheduledJobCrudRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var job = await _scheduledJobCrudRepository.GetByIdAsync(id);
        if (job == null)
        {
            return NotFound();
        }

        await _scheduledJobCrudRepository.DeleteAsync(job);
        await _scheduledJobCrudRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpPost("{jobId}/reminders")]
    public async Task<ActionResult<JobReminder>> AddReminder(Guid jobId, JobReminder reminder)
    {
        var job = await _scheduledJobCrudRepository.GetByIdAsync(jobId);
        if (job == null)
        {
            return NotFound("Scheduled job not found");
        }

        reminder.ScheduledJobId = jobId;
        var createdReminder = await _jobReminderCrudRepository.AddAsync(reminder);
        await _jobReminderCrudRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = createdReminder.Id }, createdReminder);
    }

    [HttpPut("{jobId}/complete")]
    public async Task<IActionResult> MarkAsCompleted(Guid jobId)
    {
        var job = await _scheduledJobCrudRepository.GetByIdAsync(jobId);
        if (job == null)
        {
            return NotFound();
        }

        job.CompletedDate = DateTime.UtcNow;
        await _scheduledJobCrudRepository.UpdateAsync(job);
        await _scheduledJobCrudRepository.SaveChangesAsync();
        return NoContent();
    }
} 