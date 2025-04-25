namespace Server.Client.Exceptions;

internal class ResponseDeserializationException(string? message, Exception ex) : Exception(message, ex);