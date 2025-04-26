namespace Server.Contracts.Client.Endpoints;

public class ApiRoute
{
    private string _path;
    private readonly Dictionary<string, string> _queryParams;

    public ApiRoute(string path)
    {
        _path = path;
        _queryParams = new Dictionary<string, string>();
    }

    public ApiRoute AddQueryParam(string key, string value)
    {
        _queryParams[key] = value;
        return this;
    }

    public ApiRoute AddRouteParam(string key, string value)
    {
        _path = _path.Replace(key, value);
        return this;
    }

    public override string ToString()
    {
        var query = _queryParams.Count > 0
            ? "?" + string.Join("&", _queryParams.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"))
            : "";
        return _path + query;
    }
}