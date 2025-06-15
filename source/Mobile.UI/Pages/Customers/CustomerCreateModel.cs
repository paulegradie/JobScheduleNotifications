﻿﻿﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mobile.UI.Pages.Base;
using Mobile.UI.RepositoryAbstractions;
using Server.Contracts.Endpoints.Customers.Contracts;

namespace Mobile.UI.Pages.Customers;

public partial class CustomerCreateModel : BaseViewModel
{
    private readonly ICustomerRepository _repository;

    [ObservableProperty] private string _firstName = string.Empty;

    [ObservableProperty] private string _lastName = string.Empty;

    [ObservableProperty] private string _email = string.Empty;

    [ObservableProperty] private string _phoneNumber = string.Empty;

    [ObservableProperty] private string _notes = string.Empty;


    public CustomerCreateModel(ICustomerRepository repository)
    {
        _repository = repository;
    }

    /// <summary>
    /// Clears all form fields to prepare for new customer creation
    /// </summary>
    [RelayCommand]
    public void ClearFields()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Email = string.Empty;
        PhoneNumber = string.Empty;
        Notes = string.Empty;
        ErrorMessage = string.Empty;
    }

    [RelayCommand]
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
                // Clear fields after successful creation
                ClearFields();
                // Navigate back to customer list
                await Navigation.NavigateToCustomerListAsync();
            }
            else
            {
                await ShowErrorAsync($"Failed to create customer: {result.ErrorMessage ?? "Unknown error"}");
            }
        }, "Unable to create customer.");
    }

    [RelayCommand]
    private async Task Cancel()
    {
        try
        {
            // Clear fields when cancelling
            ClearFields();
            await Navigation.NavigateToCustomerListAsync();
        }
        catch (Exception ex)
        {
            await ShowErrorAsync($"Navigation error: {ex.Message}");
        }
    }
}