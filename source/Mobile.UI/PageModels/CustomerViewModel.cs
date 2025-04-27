// CustomerViewModel.cs

using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages;
using Mobile.UI.RepositoryAbstractions;

namespace Mobile.UI.PageModels;

public partial class CustomerViewModel : ObservableObject
{
    private readonly ICustomerRepository _repository;
    private readonly INavigationRepository _navigation;

    [ObservableProperty] private CustomerId _customerId;

    [ObservableProperty] private string _firstName = string.Empty;

    [ObservableProperty] private string _lastName = string.Empty;

    [ObservableProperty] private string _email = string.Empty;

    [ObservableProperty] private string _phoneNumber = string.Empty;

    [ObservableProperty] private string _notes = string.Empty;

    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private string _title = "Customer Details";

    [ObservableProperty] private string _errorMessage = string.Empty;

    public CustomerViewModel(
        ICustomerRepository repository,
        INavigationRepository navigation)
    {
        _repository = repository;
        _navigation = navigation;
    }

    [RelayCommand]
    private async Task LoadCustomerAsync(Guid id)
    {
        if (IsBusy) return;
        IsBusy = true;
        try
        {
            var result = await _repository.GetCustomerByIdAsync(id);
            if (result is { IsSuccess: true, Value: not null })
            {
                CustomerId = result.Value.Id;
                FirstName = result.Value.FirstName;
                LastName = result.Value.LastName;
                Email = result.Value.Email;
                PhoneNumber = result.Value.PhoneNumber;
                Notes = result.Value.Notes;
            }
            else
            {
                ErrorMessage = "Failed to load customer.";
            }
        }
        catch
        {
            ErrorMessage = "Error loading customer.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task EditCustomerAsync()
    {
        if (CustomerId.Value == Guid.Empty) return;
        await _navigation.GoToAsync(
            nameof(CustomerEditPage),
            new Dictionary<string, object>
            {
                { "customerId", CustomerId.Value.ToString() }
            });
    }
}