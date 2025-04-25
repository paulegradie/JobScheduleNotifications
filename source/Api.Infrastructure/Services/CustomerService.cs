using Api.Business.Repositories;
using Api.Business.Services;
using Api.Infrastructure.DbTables.OrganizationModels;
using Api.Infrastructure.Repositories;
using Api.Infrastructure.Repositories.Mapping;
using Api.ValueTypes;
using Microsoft.AspNetCore.Identity;
using Server.Contracts.Client.Endpoints.Customers.Contracts;
using Server.Contracts.Dtos;

namespace Api.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly UserManager<ApplicationUserRecord> _userManager;
    private readonly ICrudRepository<Customer, CustomerId> _customerRepo;
    private readonly ICrudRepository<CustomerUser, CustomerId> _customerUserRepo;
    private readonly IMapperFactory _mapper;

    public CustomerService(
        UserManager<ApplicationUserRecord> userManager,
        ICrudRepository<Customer, CustomerId> customerRepo,
        ICrudRepository<CustomerUser, CustomerId> customerUserRepo,
        IMapperFactory mapper)
    {
        _userManager = userManager;
        _customerRepo = customerRepo;
        _customerUserRepo = customerUserRepo;
        _mapper = mapper;
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerRequest req, IdentityUserId currentUserId)
    {
        // 1) Create the Identity user
        var identityUser = new ApplicationUserRecord(req.Email, req.PhoneNumber);
        // TODO: You probably want to kick off an email-invite flow here rather than use a temp-pass…
        var tempPassword = $"{Guid.NewGuid():N}-ASD123!@#";
        var userResult = await _userManager.CreateAsync(identityUser, tempPassword);
        if (!userResult.Succeeded)
        {
            throw new InvalidOperationException(userResult.Errors.First().Description);
        }

        var customer = new Customer
        {
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,
            PhoneNumber = req.PhoneNumber,
            Notes = req.Notes
        };

        var createdCustomer = await _customerRepo.AddAsync(customer);

        var join = new CustomerUser
        {
            IdentityUserId = identityUser.Id,
            Customer = createdCustomer
        };
        await _customerUserRepo.AddAsync(join);

        // 4) Persist everything in one shot
        //    (Assuming both repos share the same DbContext under the covers)
        await _customerRepo.SaveChangesAsync();

        // 5) Map to DTO and return
        return createdCustomer.ToDto();
    }
}