using Api.Business.Repositories;
using Api.Infrastructure.DbTables.Jobs;
using Api.ValueTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Dtos;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/customers/{customerId:guid}/jobs/{jobId:guid}/reminders")]
    public class JobRemindersController : ControllerBase
    {
        private readonly ICrudRepository<JobReminder, JobReminderId> _jobReminderRepo;
        private readonly ICrudRepository<ScheduledJobDefinition, ScheduledJobDefinitionId> _jobDefRepo;

        public JobRemindersController(
            ICrudRepository<JobReminder, JobReminderId> jobReminderRepo,
            ICrudRepository<ScheduledJobDefinition, ScheduledJobDefinitionId> jobDefRepo)
        {
            _jobReminderRepo = jobReminderRepo;
            _jobDefRepo = jobDefRepo;
        }

        // GET: api/customers/{customerId}/jobs/{jobId}/reminders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobReminderDto>>> List(
            [FromRoute] Guid customerId,
            [FromRoute] Guid jobId,
            [FromQuery] bool? isSent)
        {
            var def = await _jobDefRepo.GetByIdAsync(new ScheduledJobDefinitionId(jobId));
            if (def == null || def.CustomerId != new CustomerId(customerId))
                return NotFound();

            // flatten reminders across all occurrences
            var reminders = def.JobOccurrences
                .SelectMany(o => o.JobReminders)
                .AsQueryable();
            if (isSent.HasValue)
                reminders = reminders.Where(r => r.IsSent == isSent.Value);

            var dtos = reminders
                .OrderBy(r => r.ReminderDateTime)
                .Select(r => new JobReminderDto(r));

            return Ok(dtos);
        }

        // GET: api/customers/{customerId}/jobs/{jobId}/reminders/{id}
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<JobReminderDto>> Get(
            [FromRoute] Guid customerId,
            [FromRoute] Guid jobId,
            [FromRoute] Guid id)
        {
            var def = await _jobDefRepo.GetByIdAsync(new ScheduledJobDefinitionId(jobId));
            if (def == null || def.CustomerId != new CustomerId(customerId))
                return NotFound();

            var reminder = def.JobOccurrences
                .SelectMany(o => o.JobReminders)
                .FirstOrDefault(r => r.Id == new JobReminderId(id));
            if (reminder == null)
                return NotFound();

            return Ok(new JobReminderDto(reminder));
        }

        // POST: api/customers/{customerId}/jobs/{jobId}/reminders
        [HttpPost]
        public async Task<ActionResult<JobReminderDto>> Create(
            [FromRoute] Guid customerId,
            [FromRoute] Guid jobId,
            [FromBody] CreateJobReminderDto dto)
        {
            var def = await _jobDefRepo.GetByIdAsync(new ScheduledJobDefinitionId(jobId));
            if (def == null || def.CustomerId != new CustomerId(customerId))
                return NotFound("Scheduled job definition not found");

            var reminder = new JobReminder
            {
                Id = new JobReminderId(Guid.NewGuid()),
                ScheduledJobId = new ScheduledJobDefinitionId(jobId),
                ReminderDateTime = dto.ReminderTime,
                Message = dto.Message,
            };

            var created = await _jobReminderRepo.AddAsync(reminder);
            await _jobReminderRepo.SaveChangesAsync();

            return CreatedAtAction(
                nameof(Get),
                new { customerId, jobId, id = created.Id.Value },
                new JobReminderDto(created));
        }

        // PUT: api/customers/{customerId}/jobs/{jobId}/reminders/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(
            [FromRoute] Guid customerId,
            [FromRoute] Guid jobId,
            [FromRoute] Guid id,
            [FromBody] UpdateJobReminderDto dto)
        {
            var def = await _jobDefRepo.GetByIdAsync(new ScheduledJobDefinitionId(jobId));
            if (def == null || def.CustomerId != new CustomerId(customerId))
                return NotFound();

            var reminder = def.JobOccurrences
                .SelectMany(o => o.JobReminders)
                .FirstOrDefault(r => r.Id == new JobReminderId(id));
            if (reminder == null)
                return NotFound();

            reminder.ReminderDateTime = dto.ReminderTime;
            reminder.Message = dto.Message;

            await _jobReminderRepo.UpdateAsync(reminder);
            await _jobReminderRepo.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/customers/{customerId}/jobs/{jobId}/reminders/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid customerId,
            [FromRoute] Guid jobId,
            [FromRoute] Guid id)
        {
            var def = await _jobDefRepo.GetByIdAsync(new ScheduledJobDefinitionId(jobId));
            if (def == null || def.CustomerId != new CustomerId(customerId))
                return NotFound();

            var reminder = def.JobOccurrences
                .SelectMany(o => o.JobReminders)
                .FirstOrDefault(r => r.Id == new JobReminderId(id));
            if (reminder == null)
                return NotFound();

            await _jobReminderRepo.DeleteAsync(reminder);
            await _jobReminderRepo.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/customers/{customerId}/jobs/{jobId}/reminders/{id}/ack
        [HttpPatch("{id:guid}/ack")]
        public async Task<IActionResult> Acknowledge(
            [FromRoute] Guid customerId,
            [FromRoute] Guid jobId,
            [FromRoute] Guid id)
        {
            var def = await _jobDefRepo.GetByIdAsync(new ScheduledJobDefinitionId(jobId));
            if (def == null || def.CustomerId != new CustomerId(customerId))
                return NotFound();

            var reminder = def.JobOccurrences
                .SelectMany(o => o.JobReminders)
                .FirstOrDefault(r => r.Id == new JobReminderId(id));
            if (reminder == null)
                return NotFound();

            reminder.IsSent = true;
            reminder.SentDate = DateTime.UtcNow;

            await _jobReminderRepo.UpdateAsync(reminder);
            await _jobReminderRepo.SaveChangesAsync();

            return NoContent();
        }
    }
}