using Api.Infrastructure.DbTables;
using JobScheduleNotifications.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BusinessOwnersController : ControllerBase
{
    private readonly IRepository<BusinessOwner> _businessOwnerRepository;

    public BusinessOwnersController(IRepository<BusinessOwner> businessOwnerRepository)
    {
        _businessOwnerRepository = businessOwnerRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BusinessOwner>>> GetAll()
    {
        var businessOwners = await _businessOwnerRepository.GetAllAsync();
        return Ok(businessOwners);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BusinessOwner>> GetById(Guid id)
    {
        var businessOwner = await _businessOwnerRepository.GetByIdAsync(id);
        if (businessOwner == null)
        {
            return NotFound();
        }
        return Ok(businessOwner);
    }

    [HttpPost]
    public async Task<ActionResult<BusinessOwner>> Create(BusinessOwner businessOwner)
    {
        var createdBusinessOwner = await _businessOwnerRepository.AddAsync(businessOwner);
        await _businessOwnerRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = createdBusinessOwner.Id }, createdBusinessOwner);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, BusinessOwner businessOwner)
    {
        if (id != businessOwner.Id)
        {
            return BadRequest();
        }

        var existingBusinessOwner = await _businessOwnerRepository.GetByIdAsync(id);
        if (existingBusinessOwner == null)
        {
            return NotFound();
        }

        await _businessOwnerRepository.UpdateAsync(businessOwner);
        await _businessOwnerRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var businessOwner = await _businessOwnerRepository.GetByIdAsync(id);
        if (businessOwner == null)
        {
            return NotFound();
        }

        await _businessOwnerRepository.DeleteAsync(businessOwner);
        await _businessOwnerRepository.SaveChangesAsync();
        return NoContent();
    }
} 