using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Business.Features.ScheduledJobs;
using Api.Infrastructure.Data;
using Api.ValueTypes;
using Api.ValueTypes.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Client.Endpoints.Customers.Contracts;
using Server.Contracts.Client.Endpoints.ScheduledJobs;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    public class ScheduledJobsController : ControllerBase
    {
        private readonly IJobDefinitionRepository _repo;
        private readonly IRecurrenceCalculator _calculator;
        private readonly IJobSchedulingService _scheduler;
        private readonly AppDbContext _uow;

        public ScheduledJobsController(
            IJobDefinitionRepository repo,
            IRecurrenceCalculator calculator,
            IJobSchedulingService scheduler,
            AppDbContext uow)
        {
            _repo = repo;
            _calculator = calculator;
            _scheduler = scheduler;
            _uow = uow;
        }

        [HttpGet(ListJobDefinitionsByCustomerIdRequest.Route)]
        public async Task<ActionResult<IEnumerable<ScheduledJobDefinitionDto>>> ListDefinitions([FromRoute] CustomerId customerId)
        {
            var defs = await _repo.ListByCustomerAsync(customerId);
            var dtos = defs.Select(d => new ScheduledJobDefinitionDto(d));
            return Ok(dtos);
        }

        // GET: api/customers/{customerId}/jobs/{jobId}
        [HttpGet(GetScheduledJobDefinitionByIdRequest.Route)]
        public async Task<ActionResult<GetScheduledJobDefinitionByIdResponse>> GetDefinition([FromRoute] CustomerId customerId, [FromRoute] ScheduledJobDefinitionId jobId)
        {
            var def = await _repo.GetAsync(jobId);
            if (def == null || def.CustomerId != customerId)
                return NotFound();

            return Ok(new GetScheduledJobDefinitionByIdResponse(def.ToDto()));
        }

        // GET: api/customers/{customerId}/jobs/{jobId}/next
        [HttpGet(GetNextScheduledJobRunRequest.Route)]
        public async Task<ActionResult<DateTime>> GetNextRun([FromRoute] CustomerId customerId, [FromRoute] ScheduledJobDefinitionId jobId)
        {
            var def = await _repo.GetAsync(jobId);
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
        [HttpPost(CreateScheduledJobRunRequest.Route)]
        public async Task<ActionResult<CreateScheduledJobDefinitionResponse>> CreateDefinition(
            [FromRoute] Guid customerId,
            [FromBody] CreateScheduledJobRunRequest req)
        {
            var def = new ScheduledJobDefinitionDomainModel
            {
                Id = new ScheduledJobDefinitionId(Guid.NewGuid()),
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

            await _repo.AddAsync(def);
            await _uow.SaveChangesAsync();

            return new CreateScheduledJobDefinitionResponse(def.ToDto());
        }

        // PATCH: api/customers/{customerId}/jobs/{jobId}
        [HttpPatch("{jobId:guid}")]
        public async Task<IActionResult> UpdateDefinition(
            [FromRoute] Guid customerId,
            [FromRoute] Guid jobId,
            [FromBody] UpdateJobDefinitionRequest req)
        {
            var def = await _repo.GetAsync(new ScheduledJobDefinitionId(jobId));
            if (def == null || def.CustomerId != new CustomerId(customerId))
                return NotFound();

            // apply changes
            def.Title = req.Title;
            def.Description = req.Description;
            def.AnchorDate = req.AnchorDate ?? def.AnchorDate;
            def.Pattern.Frequency = req.Frequency;
            def.Pattern.Interval = req.Interval;
            def.Pattern.WeekDays = req.DaysOfWeek;
            def.Pattern.DayOfMonth = req.DayOfMonth;
            def.Pattern.CronExpression = req.CronExpression;

            await _repo.UpdateAsync(def);
            await _uow.CommitAsync();

            return NoContent();
        }
    }
}