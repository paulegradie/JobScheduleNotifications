using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Business.Services;
using Api.Infrastructure.Data;
using Api.ValueTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NCrontab;
using Server.Contracts.Cron;
using Server.Contracts.Endpoints.ScheduledJobs.Contracts;

namespace Api.Controllers;

[Authorize]
[ApiController]
public class ScheduledJobController : ControllerBase
{
    private readonly IScheduledJobDefinitionRepository _scheduledJobDefinitionRepository;
    private readonly IRecurrenceCalculator _calculator;
    private readonly AppDbContext _uow;

    public ScheduledJobController(
        IScheduledJobDefinitionRepository scheduledJobDefinitionRepository,
        IRecurrenceCalculator calculator,
        AppDbContext uow)
    {
        _scheduledJobDefinitionRepository = scheduledJobDefinitionRepository;
        _calculator = calculator;
        _uow = uow;
    }

    [HttpGet(ListJobDefinitionsByCustomerIdRequest.Route)]
    public async Task<ActionResult<ListJobDefinitionsByCustomerIdResponse>> ListDefinitions([FromRoute] CustomerId customerId)
    {
        var defs = await _scheduledJobDefinitionRepository.ListByCustomerAsync(customerId);
        var dtos = defs.Select(d => d.ToDto());
        return Ok(new ListJobDefinitionsByCustomerIdResponse(dtos));
    }

    // GET: api/customers/{customerId}/jobs/{jobId}
    [HttpGet(GetScheduledJobDefinitionByIdRequest.Route)]
    public async Task<ActionResult<GetScheduledJobDefinitionByIdResponse>> GetDefinition([FromRoute] CustomerId customerId, [FromRoute] ScheduledJobDefinitionId jobDefinitionId)
    {
        var def = await _scheduledJobDefinitionRepository.GetAsync(jobDefinitionId);
        if (def == null || def.CustomerId != customerId)
            return NotFound();

        var dto = def.ToDto();
        return Ok(new GetScheduledJobDefinitionByIdResponse(def.ToDto()));
    }

    // GET: api/customers/{customerId}/jobs/{jobId}/next
    [HttpGet(GetNextScheduledJobRunRequest.Route)]
    public async Task<ActionResult<DateTime>> GetNextRun([FromRoute] CustomerId customerId, [FromRoute] ScheduledJobDefinitionId jobDefinitionId)
    {
        var def = await _scheduledJobDefinitionRepository.GetAsync(jobDefinitionId);
        if (def == null || def.CustomerId != new CustomerId(customerId))
            return NotFound();

        var cronSchedule = new CronSchedule(CrontabSchedule.Parse(def.CronExpression));

        var next = _calculator.GetNextOccurrence(cronSchedule, def.AnchorDate);
        return Ok(new GetNextScheduledJobRunResponse(next));
    }

    // POST: api/customers/{customerId}/jobs
    [HttpPost(CreateScheduledJobDefinitionRequest.Route)]
    public async Task<ActionResult<CreateScheduledJobDefinitionResponse>> CreateDefinition(
        [FromRoute] CustomerId customerId,
        [FromBody] CreateScheduledJobDefinitionRequest req)
    {
        var def = new ScheduledJobDefinitionDomainModel
        {
            ScheduledJobDefinitionId = new ScheduledJobDefinitionId(Guid.NewGuid()),
            CustomerId = new CustomerId(customerId),
            Title = req.Title,
            Description = req.Description,
            AnchorDate = req.AnchorDate,
            CronExpression = req.CronExpression,
            JobOccurrences = []
        };

        await _scheduledJobDefinitionRepository.AddAsync(def);
        await _uow.SaveChangesAsync();

        var dto = def.ToDto();
        return new CreateScheduledJobDefinitionResponse(dto);
    }

    [HttpPut(UpdateScheduledJobDefinitionRequest.Route)]
    public async Task<ActionResult<UpdateScheduledJobDefinitionResponse>> Update(
        [FromRoute] CustomerId customerId,
        [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
        [FromBody] UpdateScheduledJobDefinitionRequest req)
    {
        var def = await _scheduledJobDefinitionRepository.GetAsync(jobDefinitionId);
        if (def == null || def.CustomerId != customerId)
            return NotFound();

        // apply changes
        def.Title = req.Title;
        def.Description = req.Description;
        def.AnchorDate = req.AnchorDate;
        def.CronExpression = req.CronExpression;

        await _scheduledJobDefinitionRepository.UpdateAsync(def);

        return new UpdateScheduledJobDefinitionResponse(def.ToDto());
    }
}