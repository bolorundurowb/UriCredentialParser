namespace UriCredentialParser;

/// <summary>
/// Encapsulates connection details extracted from a URI, such as credentials, host, port, and database path.
/// </summary>
/// <param name="Scheme">Gets the URI scheme (e.g., "postgres", "mongodb").</param>
/// <param name="HostName">Gets the host name or IP address of the database server.</param>
/// <param name="UserName">Gets the username for authentication.</param>
/// <param name="Password">Gets the password for authentication.</param>
/// <param name="DatabasePath">Gets the database name or path extracted from the URI.</param>
/// <param name="Port">Gets the port number which the database server is listening on.</param>
/// <param name="AdditionalQueryParameters">Gets additional query parameters parsed from the URI query string.</param>
public record ConnectionParameters(
    string? Scheme,
    string? HostName,
    string? UserName,
    string? Password,
    string? DatabasePath,
    int? Port,
    Dictionary<string, string>? AdditionalQueryParameters)
{
    /// <summary>
    /// Combines additional query parameters into a single query string by concatenating
    /// key-value pairs with an equals sign ('=') and separating them with an ampersand ('&').
    /// </summary>
    /// <returns>
    /// A string representing the combined query parameters if the AdditionalQueryParameters
    /// property contains any key-value pairs; otherwise, null if no additional parameters are present.
    /// </returns>
    public string? ComposeAdditionalQueryParameters() => AdditionalQueryParameters == null
        ? null
        : string.Join("&", AdditionalQueryParameters.Select(kvp => $"{kvp.Key}={kvp.Value}"));
}