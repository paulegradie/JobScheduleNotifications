using Api.Business.Entities;
using Api.Business.Features.ScheduledJobs;
using Api.Business.Repositories;
using Api.Business.Services;
using Api.Infrastructure.Data;
using Api.ValueTypes;
using Api.ValueTypes.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Endpoints.ScheduledJobs.Contracts;

namespace Api.Controllers;

[Authorize]
[ApiController]
public class ScheduledJobDefinitionsController : ControllerBase
{
    private readonly IScheduledJobDefinitionRepository _scheduledJobDefinitionRepository;
    private readonly IRecurrenceCalculator _calculator;
    private readonly IJobSchedulingService _scheduler;
    private readonly AppDbContext _uow;

    public ScheduledJobDefinitionsController(
        IScheduledJobDefinitionRepository scheduledJobDefinitionRepository,
        IRecurrenceCalculator calculator,
        IJobSchedulingService scheduler,
        AppDbContext uow)
    {
        _scheduledJobDefinitionRepository = scheduledJobDefinitionRepository;
        _calculator = calculator;
        _scheduler = scheduler;
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

        return Ok(new GetScheduledJobDefinitionByIdResponse(def.ToDto()));
    }

    // GET: api/customers/{customerId}/jobs/{jobId}/next
    [HttpGet(GetNextScheduledJobRunRequest.Route)]
    public async Task<ActionResult<DateTime>> GetNextRun([FromRoute] CustomerId customerId, [FromRoute] ScheduledJobDefinitionId jobDefinitionId)
    {
        var def = await _scheduledJobDefinitionRepository.GetAsync(jobDefinitionId);
        if (def == null || def.CustomerId != new CustomerId(customerId))
            return NotFound();

        var lastOcc = def.JobOccurrences
                          .OrderByDescending(o => o.OccurrenceDate)
                          .FirstOrDefault()?.OccurrenceDate
                      ?? def.AnchorDate;

        var next = _calculator.GetNextOccurrence(def.Pattern, def.AnchorDate, lastOcc);
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
            Pattern = new RecurrencePattern(
                req.Frequency,
                req.Interval,
                req.WeekDays ?? [WeekDays.Monday],
                req.DayOfMonth,
                req.CronExpression),
            JobOccurrences = new List<JobOccurrenceDomainModel>()
        };

        await _scheduledJobDefinitionRepository.AddAsync(def);
        await _uow.SaveChangesAsync();

        return new CreateScheduledJobDefinitionResponse(def.ToDto());
    }

    [HttpPut(UpdateScheduledJobDefinitionRequest.Route)]
    public async Task<ActionResult<UpdateScheduledJobDefinitionResponse>> Update(
        [FromRoute] CustomerId customerId,
        [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
        [FromBody] CreateScheduledJobDefinitionRequest req)
    {
        var def = await _scheduledJobDefinitionRepository.GetAsync(jobDefinitionId);
        if (def == null || def.CustomerId != customerId)
            return NotFound();

        // apply changes
        def.Title = req.Title;
        def.Description = req.Description;
        def.AnchorDate = req.AnchorDate;
        def.Pattern.Frequency = req.Frequency;
        def.Pattern.Interval = req.Interval;
        def.Pattern.WeekDays = req.WeekDays ?? [WeekDays.Monday];
        def.Pattern.DayOfMonth = req.DayOfMonth;
        def.Pattern.CronExpression = req.CronExpression;

        await _scheduledJobDefinitionRepository.UpdateAsync(def);

        return new UpdateScheduledJobDefinitionResponse(def.ToDto());
    }
}