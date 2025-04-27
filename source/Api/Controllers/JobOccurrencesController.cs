using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Api.Business.Repositories;
using Api.Business.Entities;
using Api.Infrastructure.Data;
using Api.ValueTypes;
using Server.Contracts.Endpoints.JobOccurence.Contracts;

namespace Api.Controllers
{
    [Authorize]
    [ApiController]
    public class JobOccurrencesController : ControllerBase
    {
        private readonly IScheduledJobDefinitionRepository _scheduledJobDefRepo;
        private readonly IJobOccurrenceRepository _occRepo;
        private readonly AppDbContext _uow;

        public JobOccurrencesController(IScheduledJobDefinitionRepository scheduledJobDefRepo, IJobOccurrenceRepository occRepo, AppDbContext uow)
        {
            _scheduledJobDefRepo = scheduledJobDefRepo;
            _occRepo = occRepo;
            _uow = uow;
        }

        // GET: …/occurrences
        [HttpGet(ListJobOccurrencesRequest.Route)]
        public async Task<ActionResult<ListJobOccurrencesResponse>> List(
            [FromRoute] CustomerId customerId,
            [FromRoute] ScheduledJobDefinitionId jobDefinitionId)
        {
            var def = await _scheduledJobDefRepo.GetAsync(jobDefinitionId);
            if (def == null || def.CustomerId != customerId)
                return NotFound();

            var occurrences = await _occRepo.ListByDefinitionAsync(jobDefinitionId);
            var dtos = occurrences.Select(o => o.ToDto()).ToList();

            return Ok(new ListJobOccurrencesResponse(dtos));
        }

        // GET: …/occurrences/{occurrenceId}
        [HttpGet(GetJobOccurrenceByIdRequest.Route)]
        public async Task<ActionResult<GetJobOccurrenceByIdResponse>> Get(
            [FromRoute] CustomerId customerId,
            [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
            [FromRoute] JobOccurrenceId jobOccurenceId)
        {
            var def = await _scheduledJobDefRepo.GetAsync(jobDefinitionId);
            if (def == null || def.CustomerId != customerId)
                return NotFound();

            var occ = await _occRepo.GetByIdAsync(jobOccurenceId);
            if (occ == null || occ.ScheduledJobDefinitionId != jobDefinitionId)
                return NotFound();

            return Ok(new GetJobOccurrenceByIdResponse(occ.ToDto()));
        }

        // POST: …/occurrences
        [HttpPost(CreateJobOccurrenceRequest.Route)]
        public async Task<ActionResult<CreateJobOccurrenceResponse>> Create(
            [FromBody] CreateJobOccurrenceRequest req,
            [FromRoute] CustomerId customerId,
            [FromRoute] ScheduledJobDefinitionId jobDefinitionId)
        {
            var def = await _scheduledJobDefRepo.GetAsync(req.JobDefinitionId);
            if (def == null || def.CustomerId != req.CustomerId)
                return NotFound();

            var domain = new JobOccurrenceDomainModel
            {
                ScheduledJobDefinitionId = req.JobDefinitionId,
                OccurrenceDate = req.OccurrenceDate
            };

            await _occRepo.AddAsync(domain);
            
            await _uow.SaveChangesAsync();

            return Ok(new CreateJobOccurrenceResponse(domain.ToDto()));
        }

        // PUT: …/occurrences/{occurrenceId}
        [HttpPut(UpdateJobOccurrenceRequest.Route)]
        public async Task<ActionResult<UpdateJobOccurrenceResponse>> Update(
            [FromBody] UpdateJobOccurrenceRequest req)
        {
            var def = await _scheduledJobDefRepo.GetAsync(req.JobDefinitionId);
            if (def == null || def.CustomerId != req.CustomerId || def.JobOccurrences.All(o => o.Id != req.JobOccurrenceId))
                return NotFound();

            def.JobOccurrences.Where(o => o.Id == req.JobOccurrenceId).ToList().ForEach(o => o.OccurrenceDate = req.OccurrenceDate);
            var occ = await _occRepo.GetByIdAsync(req.JobOccurrenceId);
            if (occ == null || occ.ScheduledJobDefinitionId != req.JobDefinitionId)
                return NotFound();
            
            occ.OccurrenceDate = req.OccurrenceDate;
            occ.ScheduledJobDefinitionId = req.JobDefinitionId;
            await _occRepo.UpdateAsync(occ);
            
            return Ok(new UpdateJobOccurrenceResponse(occ.ToDto()));
        }

        // DELETE: …/occurrences/{occurrenceId}
        [HttpDelete(DeleteJobOccurrenceRequest.Route)]
        public async Task<IActionResult> Delete(
            [FromRoute] CustomerId customerId,
            [FromRoute] ScheduledJobDefinitionId jobDefinitionId,
            [FromRoute] JobOccurrenceId jobOccurenceId)
        {
            var def = await _scheduledJobDefRepo.GetAsync(jobDefinitionId);
            if (def == null || def.CustomerId != customerId)
                return NotFound();

            var occ = await _occRepo.GetByIdAsync(jobOccurenceId);
            if (occ == null || occ.ScheduledJobDefinitionId != jobDefinitionId)
                return NotFound();

            await _occRepo.DeleteAsync(occ);

            return NoContent();
        }
    }
}
