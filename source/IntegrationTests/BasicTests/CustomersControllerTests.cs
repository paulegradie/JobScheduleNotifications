using IntegrationTests.Base;
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
}