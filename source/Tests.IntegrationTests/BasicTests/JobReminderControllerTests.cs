using Api.ValueTypes;
using IntegrationTests.Base;
using IntegrationTests.Utils;
using Server.Contracts.Endpoints.JobOccurence.Contracts;
using Server.Contracts.Endpoints.Reminders.Contracts;
using Server.Contracts.Endpoints.ScheduledJobs.Contracts;
using Shouldly;

namespace IntegrationTests.BasicTests;

public class JobReminderControllerTests : AuthenticatedIntegrationTest
{
    private CustomerId _customerId;
    private ScheduledJobDefinitionId _jobDefinitionId;
    private JobOccurrenceId _occurrenceId;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();

        // Create a customer
        var customerReq = Some.CreateCustomerRequest();
        var customerResp = await Client.Customers.CreateCustomerAsync(customerReq, CancellationToken);
        customerResp.IsSuccess.ShouldBeTrue();
        _customerId = customerResp.Value.Customer.Id;

        // Create a scheduled job definition
        var defReq = new CreateScheduledJobDefinitionRequest(
            _customerId,
            Title: "ReminderJob",
            Description: "Desc",
            CronExpression: "* * * * *",
            AnchorDate: DateTime.UtcNow
        );
        var defResp = await Client.ScheduledJobs.CreateScheduledJobDefinitionAsync(defReq, CancellationToken);
        defResp.IsSuccess.ShouldBeTrue();
        _jobDefinitionId = defResp.Value.JobDefinition.ScheduledJobDefinitionId;

        // Create a job occurrence
        var occReq = new CreateJobOccurrenceRequest(_customerId, _jobDefinitionId, DateTime.UtcNow);
        var occResp = await Client.JobOccurrences.CreateAsync(occReq, CancellationToken);
        occResp.IsSuccess.ShouldBeTrue();
        _occurrenceId = occResp.Value.Occurrence.JobOccurrenceId;
    }

    [Fact]
    public async Task ListRemindersWithNoRemindersShouldReturnEmpty()
    {
        var listReq = new ListJobRemindersRequest(_customerId, _jobDefinitionId);
        var listResp = await Client.JobReminders.ListJobRemindersAsync(listReq, CancellationToken);

        listResp.IsSuccess.ShouldBeTrue();
        listResp.Value.JobReminder.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreateReminderAndGetByIdShouldReturnReminder()
    {
        var reminderTime = DateTime.UtcNow.AddHours(1);
        var message = "Test reminder";

        var createReq = new CreateJobReminderRequest(
            CustomerId: _customerId,
            JobDefinitionId: _jobDefinitionId,
            ReminderDateTime: reminderTime,
            Message: message
        );
        var createResp = await Client.JobReminders.CreateJobReminderAsync(createReq, CancellationToken);

        createResp.IsSuccess.ShouldBeTrue();
        var created = createResp.Value.JobReminder;
        created.ReminderDateTime.ShouldBe(reminderTime);
        created.Message.ShouldBe(message);
        created.IsSent.ShouldBeFalse();
        created.JobReminderId.ShouldNotBe(default);

        // Get by ID
        var getReq = new GetJobReminderByIdRequest(_customerId, _jobDefinitionId, created.JobReminderId);
        var getResp = await Client.JobReminders.GetJobReminderByIdAsync(getReq, CancellationToken);
        getResp.IsSuccess.ShouldBeTrue();
        var fetched = getResp.Value.JobReminder;
        fetched.JobReminderId.ShouldBe(created.JobReminderId);
        fetched.ReminderDateTime.ShouldBe(reminderTime);
        fetched.Message.ShouldBe(message);
    }

    [Fact]
    public async Task UpdateReminderShouldModifyFields()
    {
        // Create
        var originalTime = DateTime.UtcNow.AddHours(2);
        var createReq = new CreateJobReminderRequest(
            _customerId, _jobDefinitionId, originalTime, "Original"
        );
        var createResp = await Client.JobReminders.CreateJobReminderAsync(createReq, CancellationToken);
        createResp.IsSuccess.ShouldBeTrue();
        var id = createResp.Value.JobReminder.JobReminderId;

        // Update
        var newTime = originalTime.AddHours(1);
        var newMessage = "Updated message";
        var updateReq = new UpdateJobReminderRequest(
            CustomerId: _customerId,
            JobDefinitionId: _jobDefinitionId,
            ReminderId: id,
            ReminderDateTime: newTime,
            Message: newMessage
        );
        var updateResp = await Client.JobReminders.UpdateJobReminderAsync(updateReq, CancellationToken);
        updateResp.IsSuccess.ShouldBeTrue();
        var updated = updateResp.Value.JobReminderDto;
        updated.JobReminderId.ShouldBe(id);
        updated.ReminderDateTime.ShouldBe(newTime);
        updated.Message.ShouldBe(newMessage);
    }

    [Fact]
    public async Task DeleteReminderShouldRemoveIt()
    {
        // Create two reminders
        var resp1 = await Client.JobReminders.CreateJobReminderAsync(
            new CreateJobReminderRequest(_customerId, _jobDefinitionId, DateTime.UtcNow.AddHours(3), "One"),
            CancellationToken);
        resp1.IsSuccess.ShouldBeTrue();
        var id1 = resp1.Value.JobReminder.JobReminderId;

        var resp2 = await Client.JobReminders.CreateJobReminderAsync(
            new CreateJobReminderRequest(_customerId, _jobDefinitionId, DateTime.UtcNow.AddHours(4), "Two"),
            CancellationToken);
        resp2.IsSuccess.ShouldBeTrue();
        var id2 = resp2.Value.JobReminder.JobReminderId;

        // Delete first
        var delResp = await Client.JobReminders.DeleteJobReminderAsync(
            new DeleteJobReminderRequest(_customerId, _jobDefinitionId, id1),
            CancellationToken);
        delResp.IsSuccess.ShouldBeTrue();

        // List should return only second
        var listResp = await Client.JobReminders.ListJobRemindersAsync(
            new ListJobRemindersRequest(_customerId, _jobDefinitionId),
            CancellationToken);
        listResp.IsSuccess.ShouldBeTrue();
        listResp.Value.JobReminder.Count().ShouldBe(1);
        listResp.Value.JobReminder.First().JobReminderId.ShouldBe(id2);
    }

    [Fact]
    public async Task AcknowledgeReminderShouldMarkSent()
    {
        // Create
        var createResp = await Client.JobReminders.CreateJobReminderAsync(
            new CreateJobReminderRequest(_customerId, _jobDefinitionId, DateTime.UtcNow.AddHours(5), "Ack this"),
            CancellationToken);
        createResp.IsSuccess.ShouldBeTrue();
        var id = createResp.Value.JobReminder.JobReminderId;

        // Acknowledge
        var ackResp = await Client.JobReminders.AcknowledgeJobReminderAsync(
            new AcknowledgeJobReminderRequest(_customerId, _jobDefinitionId, id),
            CancellationToken);
        ackResp.IsSuccess.ShouldBeTrue();
        var acked = ackResp.Value.Reminder;
        acked.JobReminderId.ShouldBe(id);
        acked.IsSent.ShouldBeTrue();
        acked.SentDate.ShouldNotBeNull();

        // List with isSent=true should include it
        var listTrue = await Client.JobReminders.ListJobRemindersAsync(
            new ListJobRemindersRequest(_customerId, _jobDefinitionId, true),
            CancellationToken);
        listTrue.Value.JobReminder.ShouldContain(r => r.JobReminderId == id);

        // List with isSent=false should not include it
        var listFalse = await Client.JobReminders.ListJobRemindersAsync(
            new ListJobRemindersRequest(_customerId, _jobDefinitionId, false),
            CancellationToken);
        listFalse.Value.JobReminder.ShouldNotContain(r => r.JobReminderId == id);
    }
}