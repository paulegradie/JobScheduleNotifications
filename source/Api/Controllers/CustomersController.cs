using Api.Business.Interfaces;
using Api.Infrastructure.DbTables;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.Infrastructure.Repositories;
using Api.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Client.Endpoints.Customers.Contracts;

namespace Api.Controllers;

[Authorize]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICrudRepository<Customer> _customerCrudRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapperFactory _mapper;

    public CustomersController(
        ICrudRepository<Customer> customerCrudRepository,
        ICurrentUserService currentUserService,
        IMapperFactory mapper)
    {
        _customerCrudRepository = customerCrudRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    [HttpGet(GetCustomersRequest.Route)]
    public async Task<ActionResult<List<GetCustomersResponse>>> GetAll()
    {
        var customers = await _customerCrudRepository.GetAllAsync();
        var response = customers.Select(x => x.ToDto());
        return Ok(new GetCustomersResponse(response));
    }

    [HttpGet(GetCustomerByIdRequest.Route)]
    public async Task<ActionResult<Customer>> GetById([FromRoute] Guid id)
    {
        var customer = await _customerCrudRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost(CreateCustomerRequest.Route)]
    public async Task<ActionResult<Customer>> Create(CreateCustomerRequest customer)
    {
        var currentUser = _currentUserService.UserId ?? throw new InvalidOperationException("User not logged in");
        var newCustomer = new Customer
        {
            Email = customer.Email,
            Name = $"{customer.FirstName} {customer.LastName}",
            PhoneNumber = customer.PhoneNumber,
            ScheduledJobs = { }
        };
        var createdCustomer = await _customerCrudRepository.AddAsync(newCustomer);
        await _customerCrudRepository.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = createdCustomer.Id }, createdCustomer);
    }

    [HttpPut(UpdateCustomerRequest.Route)]
    public async Task<ActionResult<UpdateCustomerResponse>> Update(UpdateCustomerRequest request)
    {
        var existingCustomer = await _customerCrudRepository.GetByIdAsync(request.Id);
        if (existingCustomer == null)
        {
            return NotFound();
        }

        var updatedCustomer = await _mapper.MapAsync<Customer, Customer>(existingCustomer);
        await _customerCrudRepository.UpdateAsync(updatedCustomer);
        await _customerCrudRepository.SaveChangesAsync();
        return new UpdateCustomerResponse(updatedCustomer.ToDto());
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var customer = await _customerCrudRepository.GetByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }

        await _customerCrudRepository.DeleteAsync(customer);
        await _customerCrudRepository.SaveChangesAsync();
        return NoContent();
    }
}