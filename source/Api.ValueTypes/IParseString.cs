namespace Api.ValueTypes;

public interface IParseString<out T>
{
    public static abstract T Parse(string s);
}