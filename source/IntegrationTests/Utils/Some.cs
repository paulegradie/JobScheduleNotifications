using Server.Contracts.Client.Endpoints.Customers.Contracts;

namespace IntegrationTests.Utils;

public static class Some
{
    private static readonly Random Rnd = new Random();

    /// <summary>
    /// Generates a random lowercase string of the given length.
    /// </summary>
    public static string String(int length = 10)
        => new string(Enumerable.Range(0, length)
            .Select(_ => (char)Rnd.Next('a', 'z' + 1))
            .ToArray());

    /// <summary>
    /// Generates a random email address.
    /// </summary>
    public static string Email()
        => $"{String(8)}@{String(5)}.com";

    /// <summary>
    /// Generates a random 10-digit phone number.
    /// </summary>
    public static string PhoneNumber()
        => string.Concat(Enumerable.Range(0, 10)
            .Select(_ => Rnd.Next(0, 10).ToString()));

    /// <summary>
    /// Generates a random “note” paragraph of a few words.
    /// </summary>
    public static string Notes(int wordCount = 8)
        => string.Join(" ",
            Enumerable.Range(0, wordCount)
                .Select(_ => String(Rnd.Next(3, 8))));

    /// <summary>
    /// Creates a fully-populated CreateCustomerRequest with random data.
    /// </summary>
    public static CreateCustomerRequest CreateCustomerRequest()
        => new CreateCustomerRequest(
            email:       Email(),
            firstName:   String(6).Capitalize(),
            lastName:    String(8).Capitalize(),
            phoneNumber: PhoneNumber(),
            notes:       Notes()
        );

    // Optional helper: capitalize the first letter of a random string
    private static string Capitalize(this string s)
        => string.IsNullOrEmpty(s)
            ? s
            : char.ToUpperInvariant(s[0]) + s.Substring(1);
}