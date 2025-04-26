using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Business.Repositories;
using Api.Business.Entities;
using Api.Infrastructure.Data;
using Api.ValueTypes;
using Server.Contracts.Client.Endpoints.Reminders.Contracts;

namespace Api.Controllers;

[Authorize]
[ApiController]
public class JobRemindersController : ControllerBase
{
    private readonly IScheduledJobDefinitionRepository _scheduledJobDefRepo;
    private readonly IJobOccurrenceRepository _occRepo;
    private readonly IJobReminderRepository _remRepo;
    private readonly AppDbContext _uow;

    public JobRemindersController(
        IScheduledJobDefinitionRepository scheduledJobDefRepo,
        IJobOccurrenceRepository occRepo,
        IJobReminderRepository remRepo,
        AppDbContext uow)
    {
        _scheduledJobDefRepo = scheduledJobDefRepo;
        _occRepo = occRepo;
        _remRepo = remRepo;
        _uow = uow;
    }

    // GET: …/reminders?isSent=true
    [HttpGet(ListJobRemindersRequest.Route)]
    public async Task<ActionResult<ListJobRemindersResponse>> List(
        [FromRoute] CustomerId customerId,
        [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
        [FromQuery] bool? isSent)
    {
        var def = await _scheduledJobDefRepo.GetAsync(jobDefinitionId);
        if (def == null || def.CustomerId != customerId)
            return NotFound();

        var reminders = await _remRepo
            .ListByJobDefinitionAsync(jobDefinitionId, isSent);

        var dtos = reminders
            .Select(r => r.ToDto())
            .ToList();

        return Ok(new ListJobRemindersResponse(dtos));
    }

    // GET: …/reminders/{reminderId}
    [HttpGet(GetJobReminderByIdRequest.Route)]
    public async Task<ActionResult<GetJobReminderByIdResponse>> Get(
        [FromRoute] CustomerId customerId,
        [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
        [FromRoute] JobReminderId jobReminderId)
    {
        var def = await _scheduledJobDefRepo.GetAsync(jobDefinitionId);
        if (def == null || def.CustomerId != customerId)
            return NotFound();

        var r = await _remRepo.GetByIdAsync(jobReminderId);
        if (r == null || r.ScheduledJobDefinitionId != jobDefinitionId)
            return NotFound();

        return Ok(new GetJobReminderByIdResponse(r.ToDto()));
    }

    // POST: …/reminders
    [HttpPost(CreateJobReminderRequest.Route)]
    public async Task<ActionResult<CreateJobReminderResponse>> Create(
        [FromBody] CreateJobReminderRequest req,
        [FromRoute] CustomerId customerId,
        [FromRoute] ScheduledJobDefinitionId jobDefinitionId)
    {
        // validate definition & customer
        var def = await _scheduledJobDefRepo.GetAsync(req.JobDefinitionId);
        if (def == null || def.CustomerId != req.CustomerId)
            return NotFound();

        // pick (for example) the most recent occurrence
        var occ = (await _occRepo.ListByDefinitionAsync(req.JobDefinitionId))
            .OrderByDescending(o => o.OccurrenceDate)
            .FirstOrDefault();

        if (occ == null)
            return BadRequest("No occurrences exist for this job definition.");

        var domain = new JobReminderDomainModel
        {
            JobOccurrenceId = occ.Id,
            ScheduledJobDefinitionId = req.JobDefinitionId,
            ReminderDateTime = req.ReminderDateTime,
            Message = req.Message,
            IsSent = false,
            SentDate = null
        };

        await _remRepo.AddAsync(domain);

        return Ok(new CreateJobReminderResponse(domain.ToDto()));
    }

    // PATCH: …/reminders/{reminderId}
    [HttpPut(UpdateJobReminderRequest.Route)]
    public async Task<ActionResult<UpdateJobReminderResponse>> Update(
        [FromBody] UpdateJobReminderRequest req)
    {
        var def = await _scheduledJobDefRepo.GetAsync(req.JobDefinitionId);
        if (def == null || def.CustomerId != req.CustomerId)
            return NotFound();

        var r = await _remRepo.GetByIdAsync(req.ReminderId);
        if (r == null || r.ScheduledJobDefinitionId != req.JobDefinitionId)
            return NotFound();

        r.ReminderDateTime = req.ReminderDateTime;
        r.Message = req.Message;

        await _remRepo.UpdateAsync(r);

        return Ok(new UpdateJobReminderResponse(r.ToDto()));
    }

    // DELETE: …/reminders/{reminderId}
    [HttpDelete(DeleteJobReminderRequest.Route)]
    public async Task<IActionResult> Delete(
        [FromRoute] CustomerId customerId,
        [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
        [FromRoute] JobReminderId jobReminderId)
    {
        var def = await _scheduledJobDefRepo.GetAsync(jobDefinitionId);
        if (def == null || def.CustomerId != customerId)
            return NotFound();

        var r = await _remRepo.GetByIdAsync(jobReminderId);
        if (r == null || r.ScheduledJobDefinitionId != jobDefinitionId)
            return NotFound();

        await _remRepo.DeleteAsync(r);

        return NoContent();
    }

    // PATCH: …/reminders/{reminderId}/ack
    [HttpPut(AcknowledgeJobReminderRequest.Route)]
    public async Task<ActionResult<AcknowledgeJobReminderResponse>> Acknowledge(
        [FromRoute] CustomerId customerId,
        [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
        [FromRoute] JobReminderId jobReminderId)
    {
        var def = await _scheduledJobDefRepo.GetAsync(jobDefinitionId);
        if (def == null || def.CustomerId != customerId)
            return NotFound();

        var r = await _remRepo.GetByIdAsync(jobReminderId);
        if (r == null || r.ScheduledJobDefinitionId != jobDefinitionId)
            return NotFound();

        r.IsSent = true;
        r.SentDate = DateTime.UtcNow;

        await _remRepo.UpdateAsync(r);

        return Ok(new AcknowledgeJobReminderResponse(r.ToDto()));
    }
}