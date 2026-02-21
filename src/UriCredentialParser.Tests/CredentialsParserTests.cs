using AwesomeAssertions;

namespace UriCredentialParser.Tests;

[TestFixture]
public class CredentialsParserTests
{
    [Test]
    public void Parse_ValidAbsoluteUri_ReturnsPopulatedConnectionParameters()
    {
        // Arrange
        var url = "postgres://admin:secret@localhost:5432/testdb?timeout=30";

        // Act
        var result = CredentialsParser.Parse(url);

        // Assert
        result.Should().NotBeNull();
        result.Scheme.Should().Be("postgres");
        result.UserName.Should().Be("admin");
        result.Password.Should().Be("secret");
        result.HostName.Should().Be("localhost");
        result.Port.Should().Be(5432);
        result.DatabasePath.Should().Be("testdb");
        result.AdditionalQueryParameters.Should().Be("?timeout=30");
    }

    [Test]
    public void Parse_UriWithoutCredentials_ReturnsEmptyCredentials()
    {
        // Arrange
        var url = "mongodb://localhost/mydb";

        // Act
        var result = CredentialsParser.Parse(url);

        // Assert
        result.UserName.Should().BeEmpty();
        result.Password.Should().BeEmpty();
        result.HostName.Should().Be("localhost");
        result.DatabasePath.Should().Be("mydb");
    }

    [Test]
    public void Parse_NullUrl_ThrowsArgumentNullException()
    {
        // Act
        Action act = () => CredentialsParser.Parse(null!);

        // Assert
        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("url");
    }

    [TestCase("")]
    [TestCase("   ")]
    public void Parse_EmptyOrWhitespaceUrl_ThrowsArgumentException(string invalidUrl)
    {
        // Act
        Action act = () => CredentialsParser.Parse(invalidUrl);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithParameterName("url");
    }

    [Test]
    public void Parse_InvalidUriFormat_ThrowsUriFormatException()
    {
        // Act
        Action act = () => CredentialsParser.Parse("not-a-valid-url");

        // Assert
        act.Should().Throw<UriFormatException>();
    }
}