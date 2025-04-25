using Api.ValueTypes;
using IntegrationTests.Base;
using IntegrationTests.Utils;
using Server.Contracts.Client.Endpoints.Auth;
using Server.Contracts.Client.Endpoints.Auth.Contracts;
using Server.Contracts.Client.Endpoints.Customers.Contracts;
using Shouldly;

namespace IntegrationTests.BasicTests;

public class CustomersControllerTests : AuthenticatedIntegrationTest
{
    [Fact]
    public async Task ListCustomersWithNoCustomersShouldReturnEmptyList()
    {
        var response = await Client.Customers.GetCustomersAsync(new GetCustomersRequest(), CancellationToken);

        response.IsSuccess.ShouldBeTrue();
        response.Value.ShouldNotBeNull();
        response.Value.Customers.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetCustomerIdWithCustomerShouldReturnCustomer()
    {
        var newCustomer = Some.CreateCustomerRequest();
        var response = await Client.Customers.CreateCustomerAsync(newCustomer, CancellationToken);
        response.IsSuccess.ShouldBeTrue();
        response.Value.ShouldNotBeNull();
        response.Value.Customer.ShouldNotBeNull();

        var result = await Client.Customers.GetCustomerByIdAsync(new GetCustomerByIdRequest(response.Value.Customer.Id), CancellationToken);
        result.Value.ShouldNotBeNull();
        result.Value.Customer.PhoneNumber.ShouldBe(newCustomer.PhoneNumber);
    }

    [Fact]
    public async Task GetCustomerByIdWithNoCustomersShouldReturnEmptyResponse()
    {
        var result = await Client.Customers.GetCustomerByIdAsync(new GetCustomerByIdRequest(CustomerId.New()), CancellationToken);
        result.Value.ShouldBeNull();
    }

    [Fact]
    public async Task CreateCustomerAndListCustomersShouldReturnCustomer()
    {
        var newCustomer = new CreateCustomerRequest(
            email: "paulg@gmail.com",
            firstName: "Paul",
            lastName: "Gradie",
            phoneNumber: "8607530449",
            notes: "This guy is a real piece of work"
        );

        var response = await Client.Customers.CreateCustomerAsync(newCustomer, CancellationToken);

        response.IsSuccess.ShouldBeTrue();
        response.Value.ShouldNotBeNull();
        response.Value.Customer.ShouldNotBeNull();
    }

    [Fact]
    public async Task ListAllCustomersShouldReturnAllCustomers()
    {
        for (var i = 0; i < 10; i++)
        {
            var newCustomer = Some.CreateCustomerRequest();
            await Client.Customers.CreateCustomerAsync(newCustomer, CancellationToken);
        }

        var response = await Client.Customers.GetCustomersAsync(new GetCustomersRequest(), CancellationToken);
        response.IsSuccess.ShouldBeTrue();
        response.Value.ShouldNotBeNull();
        response.Value.AnyCustomers.ShouldBeTrue();
        response.Value.Customers.Count().ShouldBe(10);
    }

    [Fact]
    public async Task DeleteCustomerShouldResultInDeletedCustomer()
    {
        var newCustomer = Some.CreateCustomerRequest();
        var response = await Client.Customers.CreateCustomerAsync(newCustomer, CancellationToken);

        var otherCustomer = Some.CreateCustomerRequest();
        var response2 = await Client.Customers.CreateCustomerAsync(otherCustomer, CancellationToken);

        await Client.Customers.DeleteCustomerAsync(new DeleteCustomerRequest(response.Value.Customer.Id), CancellationToken);

        var result = await Client.Customers.GetCustomersAsync(new GetCustomersRequest(), CancellationToken);
        result.IsSuccess.ShouldBeTrue();
        result.Value.ShouldNotBeNull();
        result.Value.Customers.Count().ShouldBe(1);
        result.Value.Customers.First().Id.ShouldBe(response2.Value.Customer.Id);
    }

    [Fact]
    public async Task UpdateCustomerShouldResultInUpdatedCustomer()
    {
        var newCustomer = Some.CreateCustomerRequest();
        var response = await Client.Customers.CreateCustomerAsync(newCustomer, CancellationToken);
        response.IsSuccess.ShouldBeTrue();
        response.Value.ShouldNotBeNull();
        response.Value.Customer.ShouldNotBeNull();

        const string expectedValue = "UpdatedPaul";
        var updatedCustomer = UpdateCustomerRequest
            .CreateBuilder(response.Value.Customer.Id)
            .WithFirstName(expectedValue)
            .Build();
        var result = await Client.Customers.UpdateCustomerAsync(updatedCustomer, CancellationToken);

        result.Value.ShouldNotBeNull();
        result.Value.Customer.FirstName.ShouldBe(expectedValue);
        result.Value.Customer.LastName.ShouldBe(newCustomer.LastName);
        result.Value.Customer.PhoneNumber.ShouldBe(newCustomer.PhoneNumber);
        result.Value.Customer.Notes.ShouldBe(newCustomer.Notes);
        result.Value.Customer.Id.ShouldBe(response.Value.Customer.Id);
    }

    [Fact]
    public async Task WithTwoAccountsCreateCustomerAndListCustomersShouldReturnCustomerForOneAccount()
    {
        var newCustomer = new CreateCustomerRequest(
            email: "paulgCustomer@gmail.com",
            firstName: "PaulCustomer",
            lastName: "GradieCustomer",
            phoneNumber: "8607530449",
            notes: "This guy is a real piece of work"
        );

        var response = await Client.Customers.CreateCustomerAsync(newCustomer, CancellationToken);
        response.IsSuccess.ShouldBeTrue();
        response.Value.ShouldNotBeNull();
        response.Value.Customer.ShouldNotBeNull();

        // Set up an alternate account with its own customer - checking that the user boundaries are honored
        var otherUser = new RegisterNewAdminRequest
        {
            Email = "otherOwner@gmail.com",
            Password = "TestPassword123!",
            BusinessName = "OtherBusiness",
            FirstName = "Other",
            LastName = "Owner",
            PhoneNumber = "8607532345",
            BusinessDescription = "Other Business for lawns"
        };

        var registered = await Client.Auth.RegisterAsync(otherUser, CancellationToken);
        if (!registered.IsSuccess)
        {
            throw new InvalidOperationException($"User registration failed during test setup - {registered.ErrorMessage}");
        }

        var otherToken = await Client.Auth.LoginAsync(new SignInRequest(otherUser.Email, otherUser.Password), CancellationToken);
        otherToken.Value.ShouldNotBeNull();
        Client.Http.DefaultRequestHeaders.Authorization = new("Bearer", otherToken.Value.AccessToken);

        var otherCustomer = new CreateCustomerRequest(
            email: "someOtherCustomer@gmail.com",
            firstName: "SomeOther",
            lastName: "Customer",
            phoneNumber: "8607530333",
            notes: "Amazing"
        );
        var otherResponse = await Client.Customers.CreateCustomerAsync(otherCustomer, CancellationToken);


        otherResponse.IsSuccess.ShouldBeTrue();
        otherResponse.Value.ShouldNotBeNull();
        otherResponse.Value.Customer.ShouldNotBeNull();

        // Now we should have two new users plus the two original users (two owners, two customers)
        TestDb.ApplicationUsers.ToList().Count.ShouldBe(4);

        // there should be two customers
        TestDb.Customers.ToList().Count.ShouldBe(2);

        // there should be two linktable records
        TestDb.CustomerUsers.ToList().Count.ShouldBe(2);

        // We should only be able to retrieve one of the users from within the Client context of a single owner
        var otherCustomerResponse = await Client.Customers.GetCustomersAsync(new GetCustomersRequest(), CancellationToken);
        otherCustomerResponse.IsSuccess.ShouldBeTrue();
        otherCustomerResponse.Value.ShouldNotBeNull();

        // re-login with the first user
        var originalToken = await Client.Auth.LoginAsync(new SignInRequest(TestUser.Email, TestUser.Password), CancellationToken);
        Client.Http.DefaultRequestHeaders.Authorization = new("Bearer", originalToken!.Value.AccessToken);
        var customer = await Client.Customers.GetCustomersAsync(new GetCustomersRequest(), CancellationToken);
        customer.IsSuccess.ShouldBeTrue();
        customer.Value.ShouldNotBeNull();
    }
}