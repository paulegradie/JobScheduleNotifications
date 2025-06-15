using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Navigation;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Dtos;

namespace Mobile.UI.Pages.Customers;

public partial class CustomerViewModel : BaseViewModel
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
    private async Task LoadCustomerAsync(CustomerId customerId)
    {
        await RunWithSpinner(async () =>
        {
            var result = await _repository.GetCustomerByIdAsync(customerId);
            if (result is { IsSuccess: true, Value: not null })
            {
                CustomerId = result.Value.Id;
                FirstName = result.Value.FirstName;
                LastName = result.Value.LastName;
                Email = result.Value.Email;
                PhoneNumber = result.Value.PhoneNumber;
                Notes = result.Value.Notes;
            }
        }, "Failed to load customer");
        OnPropertyChanged(nameof(CustomerId));
    }

    [RelayCommand]
    private async Task EditCustomerAsync()
    {
        if (CustomerId.Value == Guid.Empty) return;
        await Navigation.NavigateToCustomerEditAsync(new CustomerParameters(CustomerId));
    }

    [RelayCommand]
    private async Task CreateJobAsync(CustomerDto? customer)
    {
        if (customer == null) return;
        await Navigation.NavigateToScheduledJobCreateAsync(new CustomerParameters(customer.Id));
    }
}