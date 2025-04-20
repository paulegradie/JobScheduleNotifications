namespace Server.Client.Exceptions;

internal class ResponseEmptyException : Exception
{
    public ResponseEmptyException(string? message) : base(message)
    {
    }
}