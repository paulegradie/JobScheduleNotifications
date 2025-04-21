using CommunityToolkit.Maui.Markup;
using Mobile.UI.PageModels;
using Mobile.UI.Pages.Base;

namespace Mobile.UI.Pages;

public sealed class ScheduleJobPage : BasePage<ScheduleJobViewModel>
{
    public ScheduleJobPage(ScheduleJobViewModel vm) : base(vm)
    {
        Title = vm.Title;

        Content = new ScrollView
        {
            Content = new Grid
            {
                Padding = 20,

                Children =
                {
                    new VerticalStackLayout
                    {
                        Spacing = 20,

                        Children =
                        {
                            Section("Select Customer",
                                new Picker()
                                    .Bind(Picker.ItemsSourceProperty, nameof(vm.Customers))
                                    .Bind(Picker.SelectedItemProperty, nameof(vm.SelectedCustomer))
                                    .Bind(Picker.SelectedItemProperty,
                                        nameof(CustomerViewModel.Name))),

                            Section("Select Date", 
                                new DatePicker()
                                    .Bind(DatePicker.DateProperty, nameof(vm.ScheduledDate))),

                            Section("Select Time",
                                new TimePicker()
                                    .Bind(TimePicker.TimeProperty, nameof(vm.ScheduledTime))),

                            Section("Job Description",
                                new Editor
                                    {
                                        Placeholder = "Enter job description",
                                        AutoSize = EditorAutoSizeOption.TextChanges,
                                        HeightRequest = 100
                                    }
                                    .Bind(Editor.TextProperty, nameof(vm.JobDescription))),

                            new Label()
                                .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
                                .TextColor(Colors.Red)
                                .CenterHorizontal()
                                .Bind(IsVisibleProperty, nameof(vm.ErrorMessage)),

                            new Button()
                                .Text("Schedule Job")
                                .BindCommand(nameof(vm.ScheduleJobCommand))
                                .Bind(Button.IsEnabledProperty, nameof(vm.IsBusy))
                                .BackgroundColor(Colors.CadetBlue)
                                .TextColor(Colors.White)
                                .FontSize(16),
                            new ActivityIndicator()
                                .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
                                .Bind(ActivityIndicator.IsVisibleProperty, nameof(vm.IsBusy))
                        }
                    }
                }
            }
        };
    }

    private static View Section(string labelText, View input) =>
        new VerticalStackLayout
        {
            Spacing = 10,
            Margin = new Thickness(0, 0, 0, 20),
            Children =
            {
                new Label()
                    .Text(labelText)
                    .Font(size: 16, bold: true),
                input
            }
        };
}