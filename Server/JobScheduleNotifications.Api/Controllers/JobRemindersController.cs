using JobScheduleNotifications.Contracts;
using JobScheduleNotifications.Core.Entities;
using JobScheduleNotifications.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduleNotifications.Api.Controllers;

[Route("job-reminders")]
public class JobRemindersController : BaseAuthenticatedApiController
{
    private readonly IRepository<JobReminder> _jobReminderRepository;
    private readonly IRepository<ScheduledJob> _scheduledJobRepository;

    public JobRemindersController(
        IRepository<JobReminder> jobReminderRepository,
        IRepository<ScheduledJob> scheduledJobRepository)
    {
        _jobReminderRepository = jobReminderRepository;
        _scheduledJobRepository = scheduledJobRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<JobReminderDto>>> GetAllJobReminders()
    {
        var reminders = await _jobReminderRepository.GetAllAsync();
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
        var reminder = await _jobReminderRepository.GetByIdAsync(id);
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
        var job = await _scheduledJobRepository.GetByIdAsync(jobId);
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
        var job = await _scheduledJobRepository.GetByIdAsync(createDto.ScheduledJobId);
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

        var createdReminder = await _jobReminderRepository.AddAsync(reminder);
        await _jobReminderRepository.SaveChangesAsync();

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
        var existingReminder = await _jobReminderRepository.GetByIdAsync(id);
        if (existingReminder == null)
        {
            return NotFound();
        }

        existingReminder.ScheduledJobId = updateDto.ScheduledJobId;
        existingReminder.ReminderTime = updateDto.ReminderTime;
        existingReminder.Message = updateDto.Message;

        await _jobReminderRepository.UpdateAsync(existingReminder);
        await _jobReminderRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteJobReminder(Guid id)
    {
        var reminder = await _jobReminderRepository.GetByIdAsync(id);
        if (reminder == null)
        {
            return NotFound();
        }

        await _jobReminderRepository.DeleteAsync(reminder);
        await _jobReminderRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpPut("{id}/sent")]
    public async Task<IActionResult> MarkJobReminderAsSent(Guid id)
    {
        var reminder = await _jobReminderRepository.GetByIdAsync(id);
        if (reminder == null)
        {
            return NotFound();
        }

        reminder.SentDate = DateTime.UtcNow;
        await _jobReminderRepository.UpdateAsync(reminder);
        await _jobReminderRepository.SaveChangesAsync();
        return NoContent();
    }
} 