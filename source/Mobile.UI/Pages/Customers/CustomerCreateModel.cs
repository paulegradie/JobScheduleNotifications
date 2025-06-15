using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Endpoints.Customers.Contracts;

namespace Mobile.UI.Pages.Customers;

public partial class CustomerCreateModel : BaseViewModel
{
    private readonly ICustomerRepository _repository;

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


    public CustomerCreateModel(ICustomerRepository repository)
    {
        _repository = repository;
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
        await RunWithSpinner(async () =>
        {
            var request = new CreateCustomerRequest(
                firstName: FirstName,
                lastName: LastName,
                email: Email,
                phoneNumber: PhoneNumber,
                notes: Notes);

            var result = await _repository.CreateCustomerAsync(request);
            if (result.IsSuccess)
            {
                await ShowSuccessAsync("Customer Created", "Returning to customer list");
            }
        }, "Unable to create customer.");

        await Navigation.NavigateToCustomerListAsync();
    }

    [RelayCommand]
    private async Task Cancel()
    {
        await ShowSuccessAsync("You Cancelled", "Returning to previous page");
        await GoBackAsync();
    }
}