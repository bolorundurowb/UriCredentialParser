namespace UriCredentialParser;

public class CredentialsParser
{
    /// <summary>
    /// Parses a given URL string into a <see cref="ConnectionParameters"/> object containing the components of the connection information.
    /// </summary>
    /// <param name="url">The URL string to parse. This must be a valid absolute URI.</param>
    /// <returns>An instance of <see cref="ConnectionParameters"/> representing the parsed connection details.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided <paramref name="url"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the provided <paramref name="url"/> is empty, contains only whitespace, or is not a valid URL.</exception>
    public static ConnectionParameters Parse(string url)
    {
        if (url == null)
            throw new ArgumentNullException(nameof(url), "Url cannot be null.");

        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Url cannot be empty or contain only whitespace characters.", nameof(url));

        var uri = new Uri(url, UriKind.Absolute);
        var auth = string.IsNullOrWhiteSpace(uri.UserInfo) ? ":" : uri.UserInfo;
        var authParts = auth.Split([':'], 2);

        int? port = uri.Port > 0 ? uri.Port : null;
        var userName = authParts[0];
        var password = authParts[1];
        var databaseName = uri.AbsolutePath.Trim('/');

        return new ConnectionParameters(uri.Scheme, uri.Host, userName, password, databaseName, port, uri.Query);
    }
}
