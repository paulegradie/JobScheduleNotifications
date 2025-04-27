using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Endpoints.Customers.Contracts;

namespace Mobile.UI.PageModels;

public partial class CreateCustomerViewModel : ObservableObject
{
    private readonly ICustomerRepository _repository;
    private readonly INavigationRepository _navigation;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanSave))]
    private string _firstName = string.Empty;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanSave))]
    private string _lastName = string.Empty;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanSave))]
    private string _email = string.Empty;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanSave))]
    private string _phoneNumber = string.Empty;

    [ObservableProperty] [NotifyPropertyChangedFor(nameof(CanSave))]
    private string _notes = string.Empty;


    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;

    public CreateCustomerViewModel(
        ICustomerRepository repository,
        INavigationRepository navigation)
    {
        _repository = repository;
        _navigation = navigation;
    }

    public bool CanSave
        => !IsBusy
           && !string.IsNullOrWhiteSpace(FirstName)
           && !string.IsNullOrWhiteSpace(LastName)
           && !string.IsNullOrWhiteSpace(Email)
           && !string.IsNullOrWhiteSpace(PhoneNumber);

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            var request = new CreateCustomerRequest(
                firstName: FirstName,
                lastName: LastName,
                email: Email,
                phoneNumber: PhoneNumber,
                notes: Notes);

            var result = await _repository.CreateCustomerAsync(request);
            if (!result.IsSuccess)
            {
                ErrorMessage = "Unable to create customer.";
                return;
            }

            await _navigation.GoToAsync(nameof(CustomersPage));
        }
        catch
        {
            ErrorMessage = "Error saving customer.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task Cancel()
    {
        var cancelled = await _navigation.ShowConfirmationAsync(
            "You cancelled!",
            $"Cancelling that shiz"
        );

        if (cancelled)
        {
            await _navigation.GoBackAsync();
        }
        else
        {
            await _navigation.ShowAlertAsync("You cancelled!", "Thats good!");
        }
    }
}