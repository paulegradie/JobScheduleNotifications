using Api.ValueTypes;
using Api.ValueTypes.Enums;
using IntegrationTests.Base;
using IntegrationTests.Utils;
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
            Frequency: Frequency.Daily,
            Interval: 1,
            WeekDays: null,
            DayOfMonth: null,
            CronExpression: null,
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
        def.Id.ShouldBe(createResp.Value.JobDefinition.Id);
    }

    [Fact]
    public async Task GetDefinitionByIdShouldReturnDefinition()
    {
        // Create
        var createReq = new CreateScheduledJobDefinitionRequest(
            _customerId,
            Title: "JobX",
            Description: "DescX",
            Frequency: Frequency.Weekly,
            Interval: 2,
            WeekDays: new[] { WeekDay.Monday, WeekDay.Wednesday },
            DayOfMonth: null,
            CronExpression: null,
            AnchorDate: DateTime.UtcNow
        );
        var createResp = await Client.ScheduledJobs.CreateScheduledJobDefinitionAsync(createReq, CancellationToken);
        createResp.IsSuccess.ShouldBeTrue();
        var id = createResp.Value.JobDefinition.Id;

        // Get
        var getReq = new GetScheduledJobDefinitionByIdRequest(_customerId, id);
        var getResp = await Client.ScheduledJobs.GetScheduledJobDefinitionByIdAsync(getReq, CancellationToken);

        getResp.IsSuccess.ShouldBeTrue();
        getResp.Value.ScheduledJobDefinitionDto.Id.ShouldBe(id);
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
            Frequency: Frequency.Monthly,
            Interval: 1,
            WeekDays: null,
            DayOfMonth: 15,
            CronExpression: null,
            AnchorDate: DateTime.UtcNow
        );
        var createResp = await Client.ScheduledJobs.CreateScheduledJobDefinitionAsync(createReq, CancellationToken);
        var id = createResp.Value.JobDefinition.Id;

        // Update
        var updatedAnchor = createReq.AnchorDate.AddHours(5);

        var updateReq = UpdateScheduledJobDefinitionRequest.CreateBuilder(_customerId, id)
            .WithTitle("After")
            .WithDescription("NewDesc")
            .WithFrequency(Frequency.Monthly)
            .WithInterval(1)
            .WithWeekDays([WeekDay.Monday])
            .WithDayOfMonth(20)
            .WithCronExpression(null!)
            .WithAnchorDate(updatedAnchor)
            .Build();

        var updateResp = await Client.ScheduledJobs.UpdateScheduledJobDefinitionAsync(updateReq, CancellationToken);
        updateResp.IsSuccess.ShouldBeTrue();
        var def = updateResp.Value.ScheduledJobDefinitionDto;
        def.Title.ShouldBe("After");
        def.Description.ShouldBe("NewDesc");
        def.AnchorDate.ShouldBe(updatedAnchor);
        def.Pattern.DayOfMonth.ShouldBe(20);
    }

    [Fact]
    public async Task GetNextRunShouldReturnCorrectNextOccurrence()
    {
        var anchor = new DateTime(2025, 4, 20, 8, 0, 0, DateTimeKind.Utc);
        var createReq = new CreateScheduledJobDefinitionRequest(
            _customerId,
            Title: "NextRunJob",
            Description: "NextRunDesc",
            Frequency: Frequency.Daily,
            Interval: 1,
            WeekDays: null,
            DayOfMonth: null,
            CronExpression: null,
            AnchorDate: anchor
        );
        var createResp = await Client.ScheduledJobs.CreateScheduledJobDefinitionAsync(createReq, CancellationToken);
        var id = createResp.Value.JobDefinition.Id;

        var nextReq = new GetNextScheduledJobRunRequest(_customerId, id);
        var nextResp = await Client.ScheduledJobs.GetNextScheduledJobRunAsync(nextReq, CancellationToken);

        nextResp.IsSuccess.ShouldBeTrue();
        // no occurrences yet, so lastOcc = anchor, next = anchor + 1 day
        nextResp.Value.NextRun.ShouldBe(anchor.AddDays(1));
    }
}