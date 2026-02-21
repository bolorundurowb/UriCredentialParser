using System.Text;
using UriCredentialParser.Enums;

namespace UriCredentialParser;

public static class Extensions
{
    /// <summary>
    /// Converts the provided <paramref name="connectionParameters"/> into a Npgsql connection string.
    /// </summary>
    /// <param name="connectionParameters">
    /// The connection parameters containing details such as hostname, username, password, and database name.
    /// </param>
    /// <param name="pooling">
    /// Specifies whether connection pooling should be enabled. Defaults to true.
    /// </param>
    /// <param name="sslMode">
    /// The SSL mode to use for the PostgreSQL connection. Defaults to <see cref="PostgresSSLMode.Prefer"/>.
    /// </param>
    /// <param name="trustServerCertificate">
    /// Specifies whether to trust the server certificate. Defaults to true.
    /// </param>
    /// <returns>
    /// A formatted Npgsql connection string based on the provided parameters.
    /// </returns>
    public static string ToNpgsqlConnectionString(this ConnectionParameters connectionParameters,
        bool pooling = true, PostgresSSLMode sslMode = PostgresSSLMode.Prefer,
        bool trustServerCertificate = true) =>
        $"User ID={connectionParameters.UserName};Password={connectionParameters.Password};Server={connectionParameters.HostName};Port={connectionParameters.Port};Database={connectionParameters.DatabasePath};Pooling={pooling.ToString().ToLowerInvariant()};SSL Mode={sslMode.ToString()};Trust Server Certificate={trustServerCertificate.ToString().ToLowerInvariant()}";

    /// <summary>
    /// Generates a MongoDB connection string and extracts the database name from the provided connection parameters.
    /// </summary>
    /// <param name="connectionParameters">
    /// The connection parameters containing details such as scheme, hostname, port, username, password, database name, and additional query parameters.
    /// </param>
    /// <returns>
    /// A tuple where the first element is the assembled MongoDB connection string and the second element is the extracted database name.
    /// </returns>
    public static (string DatabaseUrl, string? DatabaseName) ToMongoConnectionSplit(
        this ConnectionParameters connectionParameters)
    {
        string userInfo;

        if (string.IsNullOrWhiteSpace(connectionParameters.UserName) &&
            string.IsNullOrWhiteSpace(connectionParameters.UserName))
            userInfo = string.Empty;
        else
            userInfo = $"{connectionParameters.UserName}:{connectionParameters.Password}@";

        var builder =
            new StringBuilder($"{connectionParameters.Scheme}://{userInfo}{connectionParameters.HostName}");

        if (connectionParameters.Port.HasValue)
            builder.Append($":{connectionParameters.Port}");

        builder.Append(connectionParameters.AdditionalQueryParameters);

        return (builder.ToString(), connectionParameters.DatabasePath);
    }
}