

// CustomerEditViewModel.cs

using System;
using System.Threading;
using System.Threading.Tasks;
using Api.ValueTypes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Endpoints.Customers.Contracts;

namespace Mobile.UI.Pages.Customers;

public partial class CustomerEditModel : ObservableObject
{
    private readonly ICustomerRepository _repository;
    private readonly INavigationRepository _navigation;
    private CustomerId? _customerId;

    [ObservableProperty] private string _firstName    = string.Empty;
    [ObservableProperty] private string _lastName     = string.Empty;
    [ObservableProperty] private string _email        = string.Empty;
    [ObservableProperty] private string _phoneNumber  = string.Empty;
    [ObservableProperty] private string _notes        = string.Empty;
    [ObservableProperty] private bool   _isBusy;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string _title        = "Add Customer";

    public CustomerEditModel(
        ICustomerRepository repository,
        INavigationRepository navigation)
    {
        _repository = repository;
        _navigation = navigation;
    }

    public void Initialize(Guid? customerId = null)
    {
        _customerId = customerId;
        Title      = customerId.HasValue ? "Edit Customer" : "Add Customer";

        if (customerId.HasValue)
            MainThread.BeginInvokeOnMainThread(async () => await LoadCustomerAsync(customerId.Value));
    }

    [RelayCommand]
    private async Task LoadCustomerAsync(Guid id)
    {
        IsBusy = true;
        try
        {
            var result = await _repository.GetCustomerByIdAsync(id);
            if (result.IsSuccess && result.Value != null)
            {
                FirstName   = result.Value.FirstName;
                LastName    = result.Value.LastName;
                Email       = result.Value.Email;
                PhoneNumber = result.Value.PhoneNumber;
                Notes       = result.Value.Notes;
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
        finally { IsBusy = false; }
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
        IsBusy = true;
        ErrorMessage = string.Empty;

        try
        {
            if (_customerId.HasValue)
            {
                var update = UpdateCustomerRequest.CreateBuilder(_customerId.Value)
                    .WithFirstName(FirstName)
                    .WithLastName(LastName)
                    .WithEmail(Email)
                    .WithPhoneNumber(PhoneNumber)
                    .WithNotes(Notes)
                    .Build();

                await _repository.UpdateCustomerAsync(update, CancellationToken.None);
            }
            else
            {
                var createReq = new CreateCustomerRequest(
                    firstName:   FirstName,
                    lastName:    LastName,
                    email:       Email,
                    phoneNumber: PhoneNumber,
                    notes:       Notes);

                var result = await _repository.CreateCustomerAsync(createReq);
                if (result.IsSuccess)
                    _customerId = result.Value.Id;
            }

            // After save, navigate back to detail page
            await _navigation.GoBackAsync();
        }
        catch
        {
            ErrorMessage = "Failed to save customer.";
        }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task CancelAsync()
        => await _navigation.GoBackAsync();
}
