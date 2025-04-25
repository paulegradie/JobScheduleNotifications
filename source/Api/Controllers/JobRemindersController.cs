using Api.Business.Repositories;
using Api.Infrastructure.DbTables.Jobs;
using Api.ValueTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Client.Endpoints.Reminders;


namespace Api.Controllers
{
    [Authorize]
    [ApiController]
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

        // GET: api/customers/{customerId}/jobs/{jobDefinitionId}/reminders
        [HttpGet(ListJobRemindersRequest.Route)]
        public async Task<ActionResult<ListJobRemindersResponse>> List(
            [FromRoute] CustomerId customerId,
            [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
            [FromQuery] bool? isSent)
        {
            var def = await _jobDefRepo.GetByIdAsync(jobDefinitionId);
            if (def == null || def.CustomerId != customerId)
                return NotFound();

            var reminders = def.JobOccurrences
                .SelectMany(o => o.JobReminders)
                .AsQueryable();

            if (isSent.HasValue)
                reminders = reminders.Where(r => r.IsSent == isSent.Value);

            var dtos = reminders
                .OrderBy(r => r.ReminderDateTime)
                .Select(r => r.ToDto());

            return Ok(new ListJobRemindersResponse(dtos));
        }

        // GET: api/customers/{customerId}/jobs/{jobDefinitionId}/reminders/{reminderId}
        [HttpGet(GetJobReminderByIdRequest.Route)]
        public async Task<ActionResult<GetJobReminderByIdResponse>> Get(
            [FromRoute] CustomerId customerId,
            [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
            [FromRoute] JobReminderId reminderId)
        {
            var def = await _jobDefRepo.GetByIdAsync(jobDefinitionId);
            if (def == null || def.CustomerId != customerId)
                return NotFound();

            var reminder = def.JobOccurrences
                .SelectMany(o => o.JobReminders)
                .FirstOrDefault(r => r.Id == reminderId);

            if (reminder == null)
                return NotFound();

            return Ok(new GetJobReminderByIdResponse(reminder.ToDto()));
        }

        // POST: api/customers/{customerId}/jobs/{jobDefinitionId}/reminders
        [HttpPost(CreateJobReminderRequest.Route)]
        public async Task<ActionResult<CreateJobReminderResponse>> Create(
            [FromRoute] CustomerId customerId,
            [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
            [FromBody] CreateJobReminderRequest req)
        {
            var def = await _jobDefRepo.GetByIdAsync(jobDefinitionId);
            if (def == null || def.CustomerId != customerId)
                return NotFound();

            var reminder = new JobReminder
            {
                Id = new JobReminderId(Guid.NewGuid()),
                ScheduledJobDefinitionId = jobDefinitionId,
                ReminderDateTime = req.ReminderDateTime,
                Message = req.Message,
                IsSent = false
            };

            await _jobReminderRepo.AddAsync(reminder);
            await _jobReminderRepo.SaveChangesAsync();

            return Ok(new CreateJobReminderResponse(reminder.ToDto()));
        }

        // PATCH: api/customers/{customerId}/jobs/{jobDefinitionId}/reminders/{reminderId}
        [HttpPatch(UpdateJobReminderRequest.Route)]
        public async Task<ActionResult<UpdateJobReminderResponse>> Update(
            [FromRoute] CustomerId customerId,
            [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
            [FromRoute] JobReminderId reminderId,
            [FromBody] UpdateJobReminderRequest req)
        {
            var def = await _jobDefRepo.GetByIdAsync(jobDefinitionId);
            if (def == null || def.CustomerId != customerId)
                return NotFound();

            var reminder = def.JobOccurrences
                .SelectMany(o => o.JobReminders)
                .FirstOrDefault(r => r.Id == reminderId);

            if (reminder == null)
                return NotFound();

            reminder.ReminderDateTime = req.ReminderDateTime;
            reminder.Message = req.Message;

            await _jobReminderRepo.UpdateAsync(reminder);
            await _jobReminderRepo.SaveChangesAsync();

            return Ok(new UpdateJobReminderResponse(reminder.ToDto()));
        }

        // DELETE: api/customers/{customerId}/jobs/{jobDefinitionId}/reminders/{reminderId}
        [HttpDelete(DeleteJobReminderRequest.Route)]
        public async Task<IActionResult> Delete(
            [FromRoute] CustomerId customerId,
            [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
            [FromRoute] JobReminderId reminderId)
        {
            var def = await _jobDefRepo.GetByIdAsync(jobDefinitionId);
            if (def == null || def.CustomerId != customerId)
                return NotFound();

            var reminder = def.JobOccurrences
                .SelectMany(o => o.JobReminders)
                .FirstOrDefault(r => r.Id == reminderId);

            if (reminder == null)
                return NotFound();

            await _jobReminderRepo.DeleteAsync(reminder);
            await _jobReminderRepo.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/customers/{customerId}/jobs/{jobDefinitionId}/reminders/{reminderId}/ack
        [HttpPatch(AcknowledgeJobReminderRequest.Route)]
        public async Task<ActionResult<AcknowledgeJobReminderResponse>> Acknowledge(
            [FromRoute] CustomerId customerId,
            [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
            [FromRoute] JobReminderId reminderId)
        {
            var def = await _jobDefRepo.GetByIdAsync(jobDefinitionId);
            if (def == null || def.CustomerId != customerId)
                return NotFound();

            var reminder = def.JobOccurrences
                .SelectMany(o => o.JobReminders)
                .FirstOrDefault(r => r.Id == reminderId);

            if (reminder == null)
                return NotFound();

            reminder.IsSent = true;
            reminder.SentDate = DateTime.UtcNow;

            await _jobReminderRepo.UpdateAsync(reminder);
            await _jobReminderRepo.SaveChangesAsync();

            return Ok(new AcknowledgeJobReminderResponse(reminder.ToDto()));
        }
    }
}
