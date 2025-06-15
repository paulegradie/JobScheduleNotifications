using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Endpoints.Customers.Contracts;

namespace Mobile.UI.Pages.Customers;

public partial class CustomerEditModel : BaseViewModel
{
    private readonly ICustomerRepository _repository;
    public CustomerId CustomerId { get; set; }

    [ObservableProperty] private string _firstName = string.Empty;
    [ObservableProperty] private string _lastName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _phoneNumber = string.Empty;
    [ObservableProperty] private string _notes = string.Empty;
    [ObservableProperty] private string _title = "Add Customer";

    public CustomerEditModel(ICustomerRepository repository)
    {
        _repository = repository;
    }


    [RelayCommand]
    private async Task LoadCustomerAsync(string customerId)
    {
        CustomerId = CustomerId.Parse(customerId);
        await RunWithSpinner(async () =>
        {
            var result = await _repository.GetCustomerByIdAsync(CustomerId);
            if (result is { IsSuccess: true, Value: not null })
            {
                FirstName = result.Value.FirstName;
                LastName = result.Value.LastName;
                Email = result.Value.Email;
                PhoneNumber = result.Value.PhoneNumber;
                Notes = result.Value.Notes;
            }
        }, "Failed to load customer");
    }

    public bool CanSave
        => !IsBusy
           && !string.IsNullOrWhiteSpace(FirstName)
           && !string.IsNullOrWhiteSpace(LastName)
           && !string.IsNullOrWhiteSpace(Email)
           && !string.IsNullOrWhiteSpace(PhoneNumber);

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveCustomerAsync()
    {
        await RunWithSpinner(async () =>
        {
            var update = UpdateCustomerRequest.CreateBuilder(CustomerId)
                .WithFirstName(FirstName)
                .WithLastName(LastName)
                .WithEmail(Email)
                .WithPhoneNumber(PhoneNumber)
                .WithNotes(Notes)
                .Build();

            await _repository.UpdateCustomerAsync(update, CancellationToken.None);
        });
    }

    [RelayCommand]
    private async Task CancelAsync()
        => await Navigation.GoBackAsync();
}