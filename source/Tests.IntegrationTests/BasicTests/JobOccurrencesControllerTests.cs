using Api.ValueTypes;
using Api.ValueTypes.Enums;
using IntegrationTests.Base;
using IntegrationTests.Utils;
using Server.Contracts.Endpoints.JobOccurence.Contracts;
using Server.Contracts.Endpoints.ScheduledJobs.Contracts;
using Shouldly;

namespace IntegrationTests.BasicTests;

public class JobOccurrencesControllerTests : AuthenticatedIntegrationTest
{
    private CustomerId _customerId;
    private ScheduledJobDefinitionId _jobDefinitionId;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // Create a customer
        var customerReq = Some.CreateCustomerRequest();
        var customerResp = await Client.Customers.CreateCustomerAsync(customerReq, CancellationToken);
        customerResp.IsSuccess.ShouldBeTrue();
        _customerId = customerResp.Value.Customer.Id;

        // Create a scheduled job definition for that customer
        var defReq = new CreateScheduledJobDefinitionRequest(
            _customerId,
            Title: "Test Job",
            Description: "Test Description",
            Frequency: Frequency.Daily,
            Interval: 1,
            WeekDays: null,
            DayOfMonth: null,
            CronExpression: null,
            AnchorDate: DateTime.UtcNow
        );

        var defResp = await Client.ScheduledJobs.CreateScheduledJobDefinitionAsync(defReq, CancellationToken);
        defResp.IsSuccess.ShouldBeTrue();
        _jobDefinitionId = defResp.Value.JobDefinition.ScheduledJobDefinitionId;
    }

    [Fact]
    public async Task ListOccurrencesWithNoOccurrencesShouldReturnEmptyList()
    {
        var listReq = new ListJobOccurrencesRequest(_customerId, _jobDefinitionId);
        var listResp = await Client.JobOccurrences.ListAsync(listReq, CancellationToken);

        listResp.IsSuccess.ShouldBeTrue();
        listResp.Value.Occurrences.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreateOccurrenceAndGetByIdShouldReturnOccurrence()
    {
        var occurrenceDate = DateTime.UtcNow;
        var createReq = new CreateJobOccurrenceRequest(_customerId, _jobDefinitionId, occurrenceDate);
        var createResp = await Client.JobOccurrences.CreateAsync(createReq, CancellationToken);

        createResp.IsSuccess.ShouldBeTrue();
        createResp.Value.Occurrence.ShouldNotBeNull();
        createResp.Value.Occurrence.OccurrenceDate.ShouldBe(occurrenceDate);

        var occDto = createResp.Value.Occurrence;

        var getReq = new GetJobOccurrenceByIdRequest(_customerId, _jobDefinitionId, occDto.Id);
        var getResp = await Client.JobOccurrences.GetAsync(getReq, CancellationToken);

        getResp.IsSuccess.ShouldBeTrue();
        getResp.Value.JobOccurrence.Id.ShouldBe(occDto.Id);
        getResp.Value.JobOccurrence.OccurrenceDate.ShouldBe(occurrenceDate);
    }

    [Fact]
    public async Task UpdateOccurrenceShouldUpdateDate()
    {
        // First create
        var originalDate = DateTime.UtcNow;
        var createReq = new CreateJobOccurrenceRequest(_customerId, _jobDefinitionId, originalDate);
        var createResp = await Client.JobOccurrences.CreateAsync(createReq, CancellationToken);
        createResp.IsSuccess.ShouldBeTrue();

        // Update
        var newDate = originalDate.AddDays(1);
        var updateReq = new UpdateJobOccurrenceRequest(
            _customerId,
            _jobDefinitionId,
            createResp.Value.Occurrence.Id,
            newDate
        );
        var updateResp = await Client.JobOccurrences.UpdateAsync(updateReq, CancellationToken);

        updateResp.IsSuccess.ShouldBeTrue();
        updateResp.Value.Occurrence.OccurrenceDate.ShouldBe(newDate);


        // Get by ID
        var getReq = new GetJobOccurrenceByIdRequest(_customerId, _jobDefinitionId, createResp.Value.Occurrence.Id);
        var getResp = await Client.JobOccurrences.GetAsync(getReq, CancellationToken);
        getResp.IsSuccess.ShouldBeTrue();
        var fetched = getResp.Value.JobOccurrence;
        fetched.Id.ShouldBe(createResp.Value.Occurrence.Id);
        fetched.OccurrenceDate.ShouldBe(newDate);
    }

    [Fact]
    public async Task DeleteOccurrenceShouldRemoveOccurrence()
    {
        // Create two occurrences
        var date1 = DateTime.UtcNow;
        var resp1 = await Client.JobOccurrences.CreateAsync(
            new CreateJobOccurrenceRequest(_customerId, _jobDefinitionId, date1),
            CancellationToken);
        resp1.IsSuccess.ShouldBeTrue();

        var date2 = date1.AddHours(1);
        var resp2 = await Client.JobOccurrences.CreateAsync(
            new CreateJobOccurrenceRequest(_customerId, _jobDefinitionId, date2),
            CancellationToken);
        resp2.IsSuccess.ShouldBeTrue();

        // Delete the first
        var deleteReq = new DeleteJobOccurrenceRequest(
            _customerId,
            _jobDefinitionId,
            resp1.Value.Occurrence.Id
        );
        var deleteResp = await Client.JobOccurrences.DeleteAsync(deleteReq, CancellationToken);
        deleteResp.IsSuccess.ShouldBeTrue();

        // List and assert only one remains
        var listReq = new ListJobOccurrencesRequest(_customerId, _jobDefinitionId);
        var listResp = await Client.JobOccurrences.ListAsync(listReq, CancellationToken);

        listResp.IsSuccess.ShouldBeTrue();
        listResp.Value.Occurrences.Count.ShouldBe(1);
        listResp.Value.Occurrences.First().Id.ShouldBe(resp2.Value.Occurrence.Id);
    }
}