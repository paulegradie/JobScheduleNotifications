using JobScheduleNotifications.Core.Entities;
using JobScheduleNotifications.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace JobScheduleNotifications.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IRepository<Customer> _customerRepository;
    private readonly IRepository<BusinessOwner> _businessOwnerRepository;

    public CustomersController(IRepository<Customer> customerRepository, IRepository<BusinessOwner> businessOwnerRepository)
    {
        _customerRepository = customerRepository;
        _businessOwnerRepository = businessOwnerRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Customer>>> GetAll()
    {
        var customers = await _customerRepository.GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetById(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        return Ok(customer);
    }

    [HttpGet("business/{businessOwnerId}")]
    public async Task<ActionResult<IEnumerable<Customer>>> GetByBusinessOwner(Guid businessOwnerId)
    {
        var businessOwner = await _businessOwnerRepository.GetByIdAsync(businessOwnerId);
        if (businessOwner == null)
        {
            return NotFound("Business owner not found");
        }

        var customers = businessOwner.Customers;
        return Ok(customers);
    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Create(Customer customer)
    {
        // Verify business owner exists
        var businessOwner = await _businessOwnerRepository.GetByIdAsync(customer.BusinessOwnerId);
        if (businessOwner == null)
        {
            return BadRequest("Business owner not found");
        }

        var createdCustomer = await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = createdCustomer.Id }, createdCustomer);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, Customer customer)
    {
        if (id != customer.Id)
        {
            return BadRequest();
        }

        var existingCustomer = await _customerRepository.GetByIdAsync(id);
        if (existingCustomer == null)
        {
            return NotFound();
        }

        await _customerRepository.UpdateAsync(customer);
        await _customerRepository.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        await _customerRepository.DeleteAsync(customer);
        await _customerRepository.SaveChangesAsync();
        return NoContent();
    }
} 