using Api.Business.Repositories.Internal;
using Api.Business.Services;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.Infrastructure.Identity;
using Api.Infrastructure.Repositories.Mapping;
using Api.Infrastructure.Services;
using Api.ValueTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Contracts.Endpoints.Customers.Contracts;

namespace Api.Controllers;

[Authorize]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICrudRepository<Customer, CustomerId> _customerCrudRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ICustomerService _customerService;
    private readonly IMapperFactory _mapper;

    public CustomersController(
        ICrudRepository<Customer, CustomerId> customerCrudRepository,
        ICurrentUserService currentUserService,
        ICustomerService customerService,
        IMapperFactory mapper)
    {
        _customerCrudRepository = customerCrudRepository;
        _currentUserService = currentUserService;
        _customerService = customerService;
        _mapper = mapper;
    }

    [Authorize(Policy = PolicyNames.OwnerPolicy)]
    [Authorize(Policy = PolicyNames.AdminPolicy)]
    [Authorize(Policy = PolicyNames.MemberPolicy)]
    [HttpGet(GetCustomersRequest.Route)]
    public async Task<ActionResult<GetCustomersResponse>> GetAll()
    {
        var customers = await _customerCrudRepository.GetAllAsync();
        var response = customers.Select(x => x.ToDto());
        return Ok(new GetCustomersResponse(response));
    }

    [Authorize(Policy = PolicyNames.OwnerPolicy)]
    [Authorize(Policy = PolicyNames.AdminPolicy)]
    [Authorize(Policy = PolicyNames.MemberPolicy)]
    [HttpGet(GetCustomerByIdRequest.Route)]
    public async Task<ActionResult<GetCustomerByIdResponse>> GetById([FromRoute] CustomerId customerId)
    {
        var customer = await _customerCrudRepository.GetByIdAsync(customerId);
        if (customer == null)
        {
            return NotFound();
        }

        return Ok(new GetCustomerByIdResponse(customer.ToDto()));
    }

    [Authorize(Policy = PolicyNames.OwnerPolicy)]
    [Authorize(Policy = PolicyNames.AdminPolicy)]
    [Authorize(Policy = PolicyNames.MemberPolicy)]
    [HttpPost(CreateCustomerRequest.Route)]
    public async Task<ActionResult<CreateCustomerResponse>> Create(CreateCustomerRequest req)
    {
        var userId = _currentUserService.UserId
                     ?? throw new InvalidOperationException("User not logged in");

        var orgId = _currentUserService.OrganizationId ?? throw new InvalidOperationException("User not assigned to an organization.");

        var dto = await _customerService.CreateCustomerAsync(req, userId, orgId);

        return Ok(new CreateCustomerResponse(dto));
    }

    [Authorize(Policy = PolicyNames.OwnerPolicy)]
    [Authorize(Policy = PolicyNames.AdminPolicy)]
    [Authorize(Policy = PolicyNames.MemberPolicy)]
    [HttpPut(UpdateCustomerRequest.Route)]
    public async Task<ActionResult<UpdateCustomerResponse>> Update([FromRoute] CustomerId customerId, [FromBody] UpdateCustomerRequest request)
    {
        var updatedCustomer = await _mapper.MapAsync<UpdateCustomerRequest, Customer>(request);
        await _customerCrudRepository.UpdateAsync(updatedCustomer);
        await _customerCrudRepository.SaveChangesAsync();
        return new UpdateCustomerResponse(updatedCustomer.ToDto());
    }

    [Authorize(Policy = PolicyNames.OwnerPolicy)]
    [Authorize(Policy = PolicyNames.AdminPolicy)]
    [Authorize(Policy = PolicyNames.MemberPolicy)]
    [HttpDelete(DeleteCustomerRequest.Route)]
    public async Task<IActionResult> Delete([FromRoute] CustomerId customerId)
    {
        var customer = await _customerCrudRepository.GetByIdAsync(customerId);
        if (customer == null)
        {
            return NotFound();
        }

        await _customerCrudRepository.DeleteAsync(customer);
        await _customerCrudRepository.SaveChangesAsync();
        return NoContent();
    }
}