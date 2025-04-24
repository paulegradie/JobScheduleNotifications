using Api.Business.Interfaces;
using Api.Infrastructure.DbTables;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Dtos;

namespace Api.Controllers;

[Route("job-reminders")]
public class JobRemindersController : BaseAuthenticatedApiController
{
    private readonly ICrudRepository<JobReminder> _jobReminderCrudRepository;
    private readonly ICrudRepository<ScheduledJob> _scheduledJobCrudRepository;

    public JobRemindersController(
        ICrudRepository<JobReminder> jobReminderCrudRepository,
        ICrudRepository<ScheduledJob> scheduledJobCrudRepository)
    {
        _jobReminderCrudRepository = jobReminderCrudRepository;
        _scheduledJobCrudRepository = scheduledJobCrudRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobReminderDto>>> GetAllJobReminders()
    {
        var reminders = await _jobReminderCrudRepository.GetAllAsync();
        var dtos = reminders.Select(r => new JobReminderDto
        {
            Id = r.Id,
            ScheduledJobId = r.ScheduledJobId,
            ReminderTime = r.ReminderTime,
            Message = r.Message,
            IsSent = r.IsSent,
            SentDate = r.SentDate
        });
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<JobReminderDto>> GetJobReminderById(Guid id)
    {
        var reminder = await _jobReminderCrudRepository.GetByIdAsync(id);
        if (reminder == null)
        {
            return NotFound();
        }

        var dto = new JobReminderDto
        {
            Id = reminder.Id,
            ScheduledJobId = reminder.ScheduledJobId,
            ReminderTime = reminder.ReminderTime,
            Message = reminder.Message,
            IsSent = reminder.IsSent,
            SentDate = reminder.SentDate
        };
        return Ok(dto);
    }

    [HttpGet("job/{jobId}")]
    public async Task<ActionResult<IEnumerable<JobReminderDto>>> GetJobRemindersByJobId(Guid jobId)
    {
        var job = await _scheduledJobCrudRepository.GetByIdAsync(jobId);
        if (job == null)
        {
            return NotFound("Scheduled job not found");
        }

        var dtos = job.Reminders.Select(r => new JobReminderDto
        {
            Id = r.Id,
            ScheduledJobId = r.ScheduledJobId,
            ReminderTime = r.ReminderTime,
            Message = r.Message,
            IsSent = r.IsSent,
            SentDate = r.SentDate
        });
        return Ok(dtos);
    }

    [HttpPost]
    public async Task<ActionResult<JobReminderDto>> CreateJobReminder(CreateJobReminderDto createDto)
    {
        // Verify scheduled job exists
        var job = await _scheduledJobCrudRepository.GetByIdAsync(createDto.ScheduledJobId);
        if (job == null)
        {
            return BadRequest("Scheduled job not found");
        }

        var reminder = new JobReminder
        {
            ScheduledJobId = createDto.ScheduledJobId,
            ReminderTime = createDto.ReminderTime,
            Message = createDto.Message
        };

        var createdReminder = await _jobReminderCrudRepository.AddAsync(reminder);
        await _jobReminderCrudRepository.SaveChangesAsync();

        var dto = new JobReminderDto
        {
            Id = createdReminder.Id,
            ScheduledJobId = createdReminder.ScheduledJobId,
            ReminderTime = createdReminder.ReminderTime,
            Message = createdReminder.Message,
            IsSent = createdReminder.IsSent,
            SentDate = createdReminder.SentDate
        };

        return CreatedAtAction(nameof(GetJobReminderById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateJobReminder(Guid id, CreateJobReminderDto updateDto)
    {
        var existingReminder = await _jobReminderCrudRepository.GetByIdAsync(id);
        if (existingReminder == null)
        {
            return NotFound();
        }

        existingReminder.ScheduledJobId = updateDto.ScheduledJobId;
        existingReminder.ReminderTime = updateDto.ReminderTime;
        existingReminder.Message = updateDto.Message;

        await _jobReminderCrudRepository.UpdateAsync(existingReminder);
        await _jobReminderCrudRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobReminder(Guid id)
    {
        var reminder = await _jobReminderCrudRepository.GetByIdAsync(id);
        if (reminder == null)
        {
            return NotFound();
        }

        await _jobReminderCrudRepository.DeleteAsync(reminder);
        await _jobReminderCrudRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/sent")]
    public async Task<IActionResult> MarkJobReminderAsSent(Guid id)
    {
        var reminder = await _jobReminderCrudRepository.GetByIdAsync(id);
        if (reminder == null)
        {
            return NotFound();
        }

        reminder.SentDate = DateTime.UtcNow;
        await _jobReminderCrudRepository.UpdateAsync(reminder);
        await _jobReminderCrudRepository.SaveChangesAsync();
        return NoContent();
    }
} 