namespace Mobile.UI.Navigation.Parameters;

public class OrganizationSettingsParameters : INavigationParameters
{
    public OrganizationSettingsParameters()
    {
    }

    public void Validate()
    {
        // No validation needed for settings page
    }

    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>();
    }
}
