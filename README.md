# Uri Credential Parser

[![Build, Test & Coverage](https://github.com/bolorundurowb/UriCredentialParser/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/UriCredentialParser/actions/workflows/build-and-test.yml) [![codecov](https://codecov.io/gh/bolorundurowb/UriCredentialParser/graph/badge.svg?token=36BZ72JVU7)](https://codecov.io/gh/bolorundurowb/UriCredentialParser)

Small .NET library for parsing **credential-in-URL** connection strings (for example `scheme://user:password@host:port/database?options`) into structured values, and for turning those values into **Npgsql** or **MongoDB**-style connection strings when you need them.

NuGet package ID: **`ciu-parser`** (assembly/namespace: `UriCredentialParser`). Targets **.NET Standard 1.6**, so it can be referenced from a wide range of .NET runtimes.

## Installation

**Package Manager (Visual Studio)**

```powershell
Install-Package ciu-parser
```

**.NET CLI**

```bash
dotnet add package ciu-parser
```

**Paket**

```bash
paket add ciu-parser
```

## Quick start

```csharp
using UriCredentialParser;

var details = CredentialsParser.Parse(
    "postgres://someuser:somepassword@somehost:381/somedatabase?sslmode=require");

// details.Scheme, HostName, Port, UserName, Password, DatabasePath, AdditionalQueryParameters
```

Add `using UriCredentialParser.Enums;` when you use `PostgresSSLMode` with `ToNpgsqlConnectionString`.

## Parsing rules

`CredentialsParser.Parse` expects a **non-empty** string that is a valid **absolute** URI (as accepted by `System.Uri`).

| Part of the URL | Mapped property |
|-----------------|-----------------|
| Scheme | `Scheme` |
| Host | `HostName` |
| Port (when `Uri.Port` is greater than zero) | `Port`; otherwise `null` |
| User info `user:password` | `UserName`, `Password` (each may be empty if omitted) |
| Absolute path (leading and trailing `/` removed) | `DatabasePath` |
| Query string, including `?` | `AdditionalQueryParameters` |

If the URL has no user info, user name and password are returned as **empty strings** (not null).

### Errors

| Input | Exception |
|-------|-----------|
| `null` | `ArgumentNullException` |
| Empty or whitespace | `ArgumentException` |
| Not a valid URI | `UriFormatException` (from `System.Uri`) |

## `ConnectionParameters`

The object returned by `Parse` (or constructed manually) has:

- **`Scheme`** – URI scheme (for example `postgres`, `mongodb`).
- **`HostName`** – Host from the URI.
- **`Port`** – `null` when the URI has no explicit port or the default port semantics apply (`Uri.Port` is not used when it is `<= 0` in the parser).
- **`UserName`** / **`Password`** – From user info; may be empty strings.
- **`DatabasePath`** – Taken from the URI path with leading and trailing `/` removed (may include further `/` segments if present in the URL).
- **`AdditionalQueryParameters`** – Full query part as stored by the parser (typically starts with `?` when present).

## PostgreSQL (Npgsql) connection string

Extension: `ConnectionParameters.ToNpgsqlConnectionString(...)`.

Optional arguments (with defaults):

| Parameter | Type | Default |
|-----------|------|---------|
| `pooling` | `bool` | `true` |
| `sslMode` | `PostgresSSLMode` | `Prefer` |
| `trustServerCertificate` | `bool` | `true` |

`PostgresSSLMode` values: `Prefer`, `Allow`, `Disable`, `Require`, `VerifyCA`, `VerifyFull`.

```csharp
using UriCredentialParser;
using UriCredentialParser.Enums;

var details = CredentialsParser.Parse("postgres://u:p@host:5432/db");
var connString = details.ToNpgsqlConnectionString(
    pooling: false,
    sslMode: PostgresSSLMode.Require,
    trustServerCertificate: false);
```

The result is a semicolon-separated connection string with keys such as `User ID`, `Password`, `Server`, `Port`, `Database`, `Pooling`, `SSL Mode`, and `Trust Server Certificate`. If `Port` is null on the parameters object, the `Port=` fragment may be empty; set `Port` explicitly when your provider requires it.

## MongoDB URL split

Extension: `ConnectionParameters.ToMongoConnectionSplit()`.

Returns `(string DatabaseUrl, string? DatabaseName)`:

- **`DatabaseUrl`** – Rebuilds `scheme://[user:password@]host[:port]` and appends **`AdditionalQueryParameters`** unchanged (so query options remain on the URL, not in the database name).
- **`DatabaseName`** – The parsed database path (may be null if none).

```csharp
using UriCredentialParser;

var details = CredentialsParser.Parse(
    "mongodb://user:password@host:27017/database-name?retryWrites=true");

var (databaseUrl, databaseName) = details.ToMongoConnectionSplit();
// databaseUrl: mongodb://user:password@host:27017?retryWrites=true
// databaseName: database-name
```

## Security

URLs often contain **secrets** (passwords, tokens). Avoid logging raw URLs or full connection strings, storing them in plain text longer than necessary, or sending them to analytics. Treat `ConnectionParameters` like any other credential-bearing object.

## Relationship to other packages

This library supersedes the narrower [PostgresConnString.NET](https://github.com/bolorundurowb/PostgresConnString.NET) and [mongo-url-parser](https://github.com/bolorundurowb/mongo-url-parser) packages with a single, shared parser and typed exports.

## Contributing

Contributions (issues and pull requests) are welcome.

**Layout**

- `src/UriCredentialParser` – library (`CredentialsParser`, `ConnectionParameters`, `Extensions`, `Enums`).
- `src/UriCredentialParser.Tests` – unit tests (NUnit).

**Build and test** (from the `src` directory):

```bash
dotnet build UriCredentialParser.slnx
dotnet test UriCredentialParser.Tests/UriCredentialParser.Tests.csproj
```

CI runs on GitHub Actions (see `.github/workflows/build-and-test.yml`). Aim for tests that cover new behavior or fixes, and keep public API changes backward compatible unless you are intentionally shipping a major release.
