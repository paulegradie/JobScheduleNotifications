using CommunityToolkit.Maui.Markup;
using Mobile.UI.PageModels;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Client;
using Server.Contracts.Client.Endpoints.Auth.Contracts;
using static CommunityToolkit.Maui.Markup.GridRowsColumns;

namespace Mobile.UI.Pages;

public sealed class HomePage : BasePage<HomePageViewModel>
{
    private readonly HomePageViewModel _vm;

    public HomePage(HomePageViewModel vm) : base(vm)
    {
        _vm = vm;
        Title = "Welcome";

        /*──────── visual tree ────────*/
        Content = new Grid
        {
            Padding = 30,
            BackgroundColor = Colors.Gray, // AppThemeBinding via resource
            RowDefinitions = Rows.Define(
                (Row.Header, Auto),
                (Row.Center, Star),
                (Row.Footer, Auto)),

            Children =
            {
                /*─── header block ───*/
                new VerticalStackLayout
                    {
                        Spacing = 10,
                        Children =
                        {
                            new Image()
                                .Source("dotnet_bot.png")
                                .Aspect(Aspect.AspectFit)
                                .SemanticDescription("dot net bot in a hovercraft number nine"),

                            new Label()
                                .Text("Welcome!")
                                .Font(size: 32, bold: true)
                                .TextColor(Colors.Black) // overridden below by resource setter
                                .CenterHorizontal(),

                            new Label()
                                .Text("Please login or create an account to continue")
                                .FontSize(16)
                                .TextColor(Colors.Gray)
                                .CenterHorizontal()
                        }
                    }
                    .Margin(new Thickness(0, 40, 0, 20))
                    .Row(Row.Header),

                /*─── buttons block ───*/
                new VerticalStackLayout
                    {
                        Spacing = 20,
                        VerticalOptions = LayoutOptions.Center,
                        Children =
                        {
                            new Button()
                                .Text("Sign In")
                                .BindCommand((HomePageViewModel vm) => vm.NavigateToLoginCommand)
                                .BackgroundColor(Colors.CadetBlue)
                                .TextColor(Colors.White)
                                .Height(50),
                            new Button()
                                .Text("Create Account")
                                .BindCommand((HomePageViewModel vm) => vm.NavigateToRegisterCommand)
                                .BackgroundColor(Colors.ForestGreen)
                                .TextColor(Colors.CadetBlue)
                                .Height(50),
                            new Button()
                                .Text("View Customers")
                                .BindCommand((HomePageViewModel vm) => vm.NavigateToCustomersCommand)
                                .BackgroundColor(Colors.SteelBlue)
                                .TextColor(Colors.White)
                                .Height(50)
                        }
                    }
                    .Row(Row.Center),

                /*─── footer ───*/
                new Label()
                    .Text("©2025 Your App Name")
                    .TextColor(Colors.Gray)
                    .CenterHorizontal()
                    .Margin(new Thickness(0, 0, 0, 20))
                    .Row(Row.Footer)
            }
        };
    }

    /* enum helpers for Grid placement */
    private enum Row
    {
        Header,
        Center,
        Footer
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
#if DEBUG
        await _vm.AutoLoginForDev();
#endif
        
        try
        {
            if (SemanticScreenReader.Default != null)
            {
                SemanticScreenReader.Announce("Welcome");
            }
        }
        catch (Exception)
        {
            System.Diagnostics.Debug.WriteLine("SemanticScreenReader is not available");
        }
    }
}