﻿using Api.ValueTypes;
using CommunityToolkit.Maui.Markup;
using Mobile.UI.Pages.Base;
using Mobile.UI.Pages.Base.QueryParamAttributes;

namespace Mobile.UI.Pages.Customers.ScheduledJobs.JobReminders;

/// <summary>
/// Composite key for loading a single reminder.
/// </summary>
public record CustomerJobAndOccurrenceReminderIds(
    CustomerId CustomerId,
    ScheduledJobDefinitionId ScheduledJobDefinitionId,
    JobOccurrenceId JobOccurrenceId,
    JobReminderId JobReminderId);

[CustomerIdQueryParam]
[ScheduledJobDefinitionIdQueryParam]
[JobOccurrenceIdQueryParam]
[JobReminderIdQueryParamQueryParam]
public sealed class JobReminderPage : BasePage<JobReminderModel>
{
    public string JobReminderId { get; set; }
    public string JobOccurrenceId { get; set; }
    public string ScheduledJobDefinitionId { get; set; }
    public string CustomerId { get; set; }

    public JobReminderPage(JobReminderModel vm) : base(vm)
    {
        Title = "Reminder Detail";

        Content = new ScrollView
        {
            Content = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 20,
                Children =
                {
                    new Label()
                        .Bind(Label.TextProperty, nameof(vm.ReminderDate), stringFormat: "Reminder: {0:MMM d, yyyy h:mm tt}")
                        .FontSize(18).Bold(),

                    new Label()
                        .Bind(Label.TextProperty, nameof(vm.SnapshottedDescription))
                        .FontSize(16)
                }
            }
        };
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        ViewModel.LoadCommand.Execute(new CustomerJobAndOccurrenceReminderIds(
            new CustomerId(Guid.Parse(CustomerId)),
            new ScheduledJobDefinitionId(Guid.Parse(ScheduledJobDefinitionId)),
            new JobOccurrenceId(Guid.Parse(JobOccurrenceId)),
            new JobReminderId(Guid.Parse(JobReminderId))
        ));
    }
}