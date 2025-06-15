namespace Mobile.UI.RepositoryAbstractions;

/// <summary>
/// Repository abstraction for displaying alerts and confirmations to the user
/// </summary>
public interface IAlertRepository
{
    /// <summary>
    /// Shows an alert dialog with a title, message, and OK button
    /// </summary>
    /// <param name="title">The title of the alert</param>
    /// <param name="message">The message to display</param>
    Task ShowAlertAsync(string title, string message);

    /// <summary>
    /// Shows a confirmation dialog with a title, message, and Yes/No buttons
    /// </summary>
    /// <param name="title">The title of the confirmation dialog</param>
    /// <param name="message">The message to display</param>
    /// <returns>True if the user selected Yes, false if they selected No</returns>
    Task<bool> ShowConfirmationAsync(string title, string message);
}
