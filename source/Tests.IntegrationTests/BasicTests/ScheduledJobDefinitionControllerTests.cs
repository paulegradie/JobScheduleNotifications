using Api.ValueTypes;
using Api.ValueTypes.Enums;
using IntegrationTests.Base;
using IntegrationTests.Utils;
using Server.Contracts.Cron;
using Server.Contracts.Endpoints.ScheduledJobs.Contracts;
using Shouldly;

namespace IntegrationTests.BasicTests;

public class ScheduledJobDefinitionControllerTests : AuthenticatedIntegrationTest
{
    private CustomerId _customerId;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // Create a test customer
        var customerReq = Some.CreateCustomerRequest();
        var custResp = await Client.Customers.CreateCustomerAsync(customerReq, CancellationToken);
        custResp.IsSuccess.ShouldBeTrue();
        _customerId = custResp.Value.Customer.Id;
    }

    [Fact]
    public async Task ListDefinitionsWithNoDefinitionsShouldReturnEmpty()
    {
        var listReq = new ListJobDefinitionsByCustomerIdRequest(_customerId);
        var listResp = await Client.ScheduledJobs.ListJobDefinitionsAsync(listReq, CancellationToken);

        listResp.IsSuccess.ShouldBeTrue();
        listResp.Value.ScheduledJobDefinitions.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreateDefinitionAndListShouldReturnDefinition()
    {
        var anchor = DateTime.UtcNow;
        var createReq = new CreateScheduledJobDefinitionRequest(
            _customerId,
            Title: "My Job",
            Description: "Desc",
            CronExpression: "* * * * *",
            AnchorDate: anchor
        );

        var createResp = await Client.ScheduledJobs.CreateScheduledJobDefinitionAsync(createReq, CancellationToken);
        createResp.IsSuccess.ShouldBeTrue();
        createResp.Value.JobDefinition.ShouldNotBeNull();
        createResp.Value.JobDefinition.Title.ShouldBe("My Job");
        createResp.Value.JobDefinition.AnchorDate.ShouldBe(anchor);

        var listReq = new ListJobDefinitionsByCustomerIdRequest(_customerId);
        var listResp = await Client.ScheduledJobs.ListJobDefinitionsAsync(listReq, CancellationToken);

        listResp.Value.ScheduledJobDefinitions.Count().ShouldBe(1);
        var def = listResp.Value.ScheduledJobDefinitions.First();
        def.ScheduledJobDefinitionId.ShouldBe(createResp.Value.JobDefinition.ScheduledJobDefinitionId);
    }

    [Fact]
    public async Task GetDefinitionByIdShouldReturnDefinition()
    {
        // Create
        var createReq = new CreateScheduledJobDefinitionRequest(
            _customerId,
            Title: "JobX",
            Description: "DescX",
            CronExpression: "* * * * *",
            AnchorDate: DateTime.UtcNow
        );
        var createResp = await Client.ScheduledJobs.CreateScheduledJobDefinitionAsync(createReq, CancellationToken);
        createResp.IsSuccess.ShouldBeTrue();
        var id = createResp.Value.JobDefinition.ScheduledJobDefinitionId;

        // Get
        var getReq = new GetScheduledJobDefinitionByIdRequest(_customerId, id);
        var getResp = await Client.ScheduledJobs.GetScheduledJobDefinitionByIdAsync(getReq, CancellationToken);

        getResp.IsSuccess.ShouldBeTrue();
        getResp.Value.ScheduledJobDefinitionDto.ScheduledJobDefinitionId.ShouldBe(id);
        getResp.Value.ScheduledJobDefinitionDto.Title.ShouldBe("JobX");
    }

    [Fact]
    public async Task UpdateDefinitionShouldUpdateFields()
    {
        // Create
        var createReq = new CreateScheduledJobDefinitionRequest(
            _customerId,
            Title: "Before",
            Description: "Old",
            CronExpression: "* * * * *",
            AnchorDate: DateTime.UtcNow
        );
        var createResp = await Client.ScheduledJobs.CreateScheduledJobDefinitionAsync(createReq, CancellationToken);
        var id = createResp.Value.JobDefinition.ScheduledJobDefinitionId;

        // Update
        var updatedAnchor = createReq.AnchorDate.AddHours(5);

        var updateReq = UpdateScheduledJobDefinitionRequest.CreateBuilder(_customerId, id)
            .WithTitle("After")
            .WithDescription("NewDesc")
            .WithCronExpression("* * * * *")
            .WithAnchorDate(updatedAnchor)
            .Build();

        var updateResp = await Client.ScheduledJobs.UpdateScheduledJobDefinitionAsync(updateReq, CancellationToken);
        updateResp.IsSuccess.ShouldBeTrue();
        var def = updateResp.Value.ScheduledJobDefinitionDto;
        def.Title.ShouldBe("After");
        def.Description.ShouldBe("NewDesc");
        def.AnchorDate.ShouldBe(updatedAnchor);
        def.CronExpression.ShouldBe("* * * * *");
    }

    [Fact]
    public async Task GetNextRunShouldReturnCorrectNextOccurrence()
    {
        var anchor = new DateTime(2025, 4, 20, 8, 0, 0, DateTimeKind.Utc);

        var schedule = FluentCron.Create()
            .AtHour(0)
            .AtMinute(0)
            .EveryDays(1)
            .Build();

        var createReq = new CreateScheduledJobDefinitionRequest(
            _customerId,
            Title: "NextRunJob",
            Description: "NextRunDesc",
            CronExpression: schedule.ToString(),
            AnchorDate: anchor
        );
        var createResp = await Client.ScheduledJobs.CreateScheduledJobDefinitionAsync(createReq, CancellationToken);
        var id = createResp.Value.JobDefinition.ScheduledJobDefinitionId;

        var nextReq = new GetNextScheduledJobRunRequest(_customerId, id);
        var nextResp = await Client.ScheduledJobs.GetNextScheduledJobRunAsync(nextReq, CancellationToken);

        nextResp.IsSuccess.ShouldBeTrue();
        // no occurrences yet, so lastOcc = anchor, next = anchor + 1 day
        nextResp.Value.NextRun.ShouldBe(anchor.AddDays(1));
    }
}