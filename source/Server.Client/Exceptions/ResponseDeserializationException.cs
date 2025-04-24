using System.Text.Json;

namespace Server.Client.Exceptions;

internal class ResponseDeserializationException : Exception
{
    public ResponseDeserializationException(string s, JsonException jsonException)
    {
        throw new NotImplementedException();
    }
}