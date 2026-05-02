using AwesomeAssertions;

namespace UriCredentialParser.Tests;

[TestFixture]
public class CredentialsParserTests
{
    [Test]
    public void Parse_ValidAbsoluteUri_ReturnsPopulatedConnectionParameters()
    {
        var url = "postgres://admin:secret@localhost:5432/testdb?timeout=30";

        var result = CredentialsParser.Parse(url);

        result.Should().NotBeNull();
        result.Scheme.Should().Be("postgres");
        result.UserName.Should().Be("admin");
        result.Password.Should().Be("secret");
        result.HostName.Should().Be("localhost");
        result.Port.Should().Be(5432);
        result.DatabasePath.Should().Be("testdb");
        result.AdditionalQueryParameters.Should().NotBeNull();
        result.AdditionalQueryParameters!["timeout"].Should().Be("30");
    }

    [Test]
    public void Parse_MultipleQueryParameters_ReturnsCorrectDictionary()
    {
        var url = "mysql://user:pass@localhost/db?timeout=30&ssl=true&mode=readonly";

        var result = CredentialsParser.Parse(url);

        result.AdditionalQueryParameters.Should().NotBeNull();
        result.AdditionalQueryParameters.Should().HaveCount(3);
        result.AdditionalQueryParameters!["timeout"].Should().Be("30");
        result.AdditionalQueryParameters!["ssl"].Should().Be("true");
        result.AdditionalQueryParameters!["mode"].Should().Be("readonly");
    }

    [Test]
    public void Parse_QueryParameterWithoutValue_ReturnsEmptyStringValue()
    {
        var url = "redis://localhost?debug";

        var result = CredentialsParser.Parse(url);

        result.AdditionalQueryParameters.Should().NotBeNull();
        result.AdditionalQueryParameters.Should().ContainKey("debug");
        result.AdditionalQueryParameters!["debug"].Should().BeEmpty();
    }

    [Test]
    public void Parse_UriWithoutCredentials_ReturnsEmptyCredentials()
    {
        var url = "mongodb://localhost/mydb";

        var result = CredentialsParser.Parse(url);

        result.UserName.Should().BeEmpty();
        result.Password.Should().BeEmpty();
        result.HostName.Should().Be("localhost");
        result.DatabasePath.Should().Be("mydb");
    }

    [Test]
    public void Parse_NullUrl_ThrowsArgumentNullException()
    {
        Action act = () => CredentialsParser.Parse(null!);

        act.Should().Throw<ArgumentNullException>()
            .WithParameterName("url");
    }

    [TestCase("")]
    [TestCase("   ")]
    public void Parse_EmptyOrWhitespaceUrl_ThrowsArgumentException(string invalidUrl)
    {
        Action act = () => CredentialsParser.Parse(invalidUrl);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("url");
    }

    [Test]
    public void Parse_InvalidUriFormat_ThrowsUriFormatException()
    {
        Action act = () => CredentialsParser.Parse("not-a-valid-url");

        act.Should().Throw<UriFormatException>();
    }
}