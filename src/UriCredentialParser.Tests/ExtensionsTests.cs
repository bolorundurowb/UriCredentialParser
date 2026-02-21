using AwesomeAssertions;
using UriCredentialParser.Enums;

namespace UriCredentialParser.Tests;

[TestFixture]
public class ExtensionsTests
{
    [Test]
    public void ToNpgsqlConnectionString_WithDefaultParameters_ReturnsExpectedString()
    {
        // Arrange
        var parameters = new ConnectionParameters("postgres", "db-server", "dbuser", "dbpass", "maindb", 5432, null);

        // Act
        var result = parameters.ToNpgsqlConnectionString();

        // Assert
        result.Should().Be("User ID=dbuser;Password=dbpass;Server=db-server;Port=5432;Database=maindb;Pooling=true;SSL Mode=Prefer;Trust Server Certificate=true");
    }

    [Test]
    public void ToNpgsqlConnectionString_WithCustomParameters_ReturnsExpectedString()
    {
        // Arrange
        var parameters = new ConnectionParameters("postgres", "127.0.0.1", "usr", "pwd", "db", 5433, null);

        // Act
        var result = parameters.ToNpgsqlConnectionString(pooling: false, sslMode: PostgresSSLMode.Require, trustServerCertificate: false);

        // Assert
        result.Should().Be("User ID=usr;Password=pwd;Server=127.0.0.1;Port=5433;Database=db;Pooling=false;SSL Mode=Require;Trust Server Certificate=false");
    }

    [Test]
    public void ToMongoConnectionSplit_WithFullCredentialsAndPort_ReturnsCorrectTuple()
    {
        // Arrange
        var parameters = new ConnectionParameters("mongodb", "mongo-cluster", "admin", "pass123", "appdb", 27017, "?retryWrites=true");

        // Act
        var (url, dbName) = parameters.ToMongoConnectionSplit();

        // Assert
        url.Should().Be("mongodb://admin:pass123@mongo-cluster:27017?retryWrites=true");
        dbName.Should().Be("appdb");
    }

    [Test]
    public void ToMongoConnectionSplit_WithoutCredentials_ReturnsCorrectTuple()
    {
        // Arrange
        var parameters = new ConnectionParameters("mongodb", "localhost", null, null, "localdb", null, null);

        // Act
        var (url, dbName) = parameters.ToMongoConnectionSplit();

        // Assert
        url.Should().Be("mongodb://localhost");
        dbName.Should().Be("localdb");
    }
}