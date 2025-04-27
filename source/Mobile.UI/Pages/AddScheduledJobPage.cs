using Api.ValueTypes.Enums;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Markup;
using Mobile.UI.PageModels;
using Mobile.UI.Pages.Base;

namespace Mobile.UI.Pages;

   public sealed class AddScheduledJobPage : BasePage<AddScheduledJobViewModel>
    {
        private readonly AddScheduledJobViewModel _vm;

        public AddScheduledJobPage(AddScheduledJobViewModel vm) : base(vm)
        {
            _vm = vm;
            Title = "Add Scheduled Job";

            Content = new ScrollView
            {
                Content = new VerticalStackLayout
                {
                    Padding = 20,
                    Spacing = 15,
                    Children =
                    {
                        Section("Customer",
                            new Picker()
                                .Bind(Picker.ItemsSourceProperty, nameof(vm.Customers))
                                .Bind(Picker.SelectedItemProperty, nameof(vm.SelectedCustomer))),

                        Section("Title",
                            new Entry()
                                .Placeholder("Job title")
                                .Bind(Entry.TextProperty, nameof(vm.Title))),

                        Section("Description",
                            new Editor { HeightRequest = 100 }
                                .Bind(Editor.TextProperty, nameof(vm.Description))),

                        Section("Anchor Date",
                            new DatePicker()
                                .Bind(DatePicker.DateProperty, nameof(vm.AnchorDate))),

                        Section("Frequency",
                            new Picker { ItemsSource = Enum.GetValues<Frequency>() }
                                .Bind(Picker.SelectedItemProperty, nameof(vm.Frequency))),

                        Section("Interval",
                            new Stepper { Minimum = 1, Maximum = 30 }
                                .Bind(Stepper.ValueProperty, nameof(vm.Interval))),

                        Section("Week Days",
                            new HorizontalStackLayout
                            {
                                Spacing = 5,
                                // you can generate CheckBoxes here bound to vm.SelectedWeekDays
                            }),

                        Section("Day of Month",
                            new Entry { Keyboard = Keyboard.Numeric }
                                .Bind(Entry.TextProperty, nameof(vm.DayOfMonth))),

                        Section("Cron Expression",
                            new Entry()
                                .Placeholder("Optional cron")
                                .Bind(Entry.TextProperty, nameof(vm.CronExpression))),

                        new Label()
                            .Bind(Label.TextProperty, nameof(vm.ErrorMessage))
                            .TextColor(Colors.Red)
                            .Bind(IsVisibleProperty, nameof(vm.ErrorMessage)),

                        new Button()
                            .Text("Create Job")
                            .BindCommand(nameof(vm.CreateJobCommand))
                            .Bind(IsEnabledProperty, nameof(vm.IsBusy),
                                  converter: new InvertedBoolConverter()),

                        new ActivityIndicator()
                            .Bind(ActivityIndicator.IsRunningProperty, nameof(vm.IsBusy))
                            .Bind(ActivityIndicator.IsVisibleProperty, nameof(vm.IsBusy)),
                    }
                }
            };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.LoadCustomersCommand.Execute(null);
        }

        static View Section(string label, View control) =>
            new VerticalStackLayout
            {
                Spacing = 2,
                Children =
                {
                    new Label().Text(label).Font(size: 14, bold: true),
                    control
                }
            };
    }