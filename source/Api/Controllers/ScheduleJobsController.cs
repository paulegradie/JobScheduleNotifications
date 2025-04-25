using Api.Business.Entities;
using Api.Business.Repositories;
using Api.Business.Features.ScheduledJobs;
using Api.ValueTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Client.Endpoints.Customers.Contracts;
using Server.Contracts.Client.Endpoints.ScheduledJobs;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/customers/{customerId:guid}/jobs")]
    public class ScheduledJobsController : ControllerBase
    {
        private readonly IJobDefinitionRepository _repo;
        private readonly IRecurrenceCalculator _calculator;
        private readonly IJobSchedulingService _scheduler;
        private readonly IUnitOfWork _uow;

        public ScheduledJobsController(
            IJobDefinitionRepository repo,
            IRecurrenceCalculator calculator,
            IJobSchedulingService scheduler,
            IUnitOfWork uow)
        {
            _repo = repo;
            _calculator = calculator;
            _scheduler = scheduler;
            _uow = uow;
        }

        // GET: api/customers/{customerId}/jobs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ScheduledJobDefinitionDto>>> ListDefinitions([FromRoute] Guid customerId)
        {
            var defs = await _repo.ListByCustomerAsync(new CustomerId(customerId));
            var dtos = defs.Select(d => new ScheduledJobDefinitionDto(d));
            return Ok(dtos);
        }

        // GET: api/customers/{customerId}/jobs/{jobId}
        [HttpGet("{jobId:guid}", Name = "GetJobDefinition")]
        public async Task<ActionResult<ScheduledJobDefinitionDto>> GetDefinition([FromRoute] Guid customerId, [FromRoute] Guid jobId)
        {
            var def = await _repo.GetAsync(new ScheduledJobDefinitionId(jobId));
            if (def == null || def.CustomerId != new CustomerId(customerId))
                return NotFound();

            return Ok(new ScheduledJobDefinitionDto(def));
        }

        // GET: api/customers/{customerId}/jobs/{jobId}/next
        [HttpGet("{jobId:guid}/next")]
        public async Task<ActionResult<DateTime>> GetNextRun([FromRoute] Guid customerId, [FromRoute] Guid jobId)
        {
            var def = await _repo.GetAsync(new ScheduledJobDefinitionId(jobId));
            if (def == null || def.CustomerId != new CustomerId(customerId))
                return NotFound();

            var lastOcc = def.Occurrences
                              .OrderByDescending(o => o.OccurrenceDate)
                              .FirstOrDefault()?.OccurrenceDate
                          ?? def.AnchorDate;

            var next = _calculator.GetNextOccurrence(def.Pattern, def.AnchorDate, lastOcc);
            return Ok(next);
        }

        // POST: api/customers/{customerId}/jobs
        [HttpPost]
        public async Task<ActionResult<ScheduledJobDefinitionDto>> CreateDefinition(
            [FromRoute] Guid customerId,
            [FromBody] CreateJobDefinitionRequest req)
        {
            var def = new ScheduledJobDefinitionDomainModel
            {
                Id = new ScheduledJobDefinitionId(Guid.NewGuid()),
                CustomerId = new CustomerId(customerId),
                Title = req.Title,
                Description = req.Description,
                AnchorDate = req.AnchorDate ?? DateTime.UtcNow,
                Pattern = new RecurrencePattern(
                    req.Frequency,
                    req.Interval,
                    req.DaysOfWeek,
                    req.DayOfMonth,
                    req.CronExpression),
                JobOccurrences = new List<JobOccurrenceDomainModel>()
            };

            await _repo.AddAsync(def);
            await _uow.CommitAsync();

            var dto = new ScheduledJobDefinitionDto(def);
            return CreatedAtRoute(
                "GetJobDefinition",
                new { customerId, jobId = def.Id.Value },
                dto);
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
            def.Pattern.DaysOfWeek = req.DaysOfWeek;
            def.Pattern.DayOfMonth = req.DayOfMonth;
            def.Pattern.CronExpression = req.CronExpression;

            await _repo.UpdateAsync(def);
            await _uow.CommitAsync();

            return NoContent();
        }
    }
}