# Uri Credential Parser

[![Build, Test & Coverage](https://github.com/bolorundurowb/UriCredentialParser/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/UriCredentialParser/actions/workflows/build-and-test.yml) [![codecov](https://codecov.io/gh/bolorundurowb/UriCredentialParser/graph/badge.svg?token=36BZ72JVU7)](https://codecov.io/gh/bolorundurowb/UriCredentialParser)

This package replaces both [PostgresConnString.NET](https://github.com/bolorundurowb/PostgresConnString.NET/tree/master) and [mongo-url-parser](https://github.com/bolorundurowb/mongo-url-parser) as they are very basic packages.

It aims to grant general .NET support for parsing and converting URLs in the form "scheme://user:password@host:port/database?connectionparameters" (also known as Credential-In-Url) and converting them to formats easily used by database service providers for .NET.

## Installation

You can install the package from NuGet:

```bash
Install-Package ciu-parser

```

or via the .NET CLI:

```bash
dotnet add package ciu-parser

```

or for Paket:

```bash
paket add ciu-parser

```

## Usage

### Parsing URLs

To parse a URL:

```csharp
using UriCredentialParser;

// ...

var details = CredentialsParser.Parse("postgres://someuser:somepassword@somehost:381/somedatabase");

```

The resulting `ConnectionParameters` object contains the following properties:

* `Scheme` - Database server scheme
* `HostName` - Database server hostname
* `Port` - Port on which to connect
* `UserName` - User with which to authenticate to the server
* `Password` - Corresponding password
* `DatabasePath` - Database name within the server
* `AdditionalQueryParameters` - Additional database parameters provided as query options

### Exports

Currently, this library allows for generating Npgsql-compatible connection strings with the following parameters:

* `pooling`: type: boolean, default: `true`
* `sslMode`: type: `PostgresSSLMode` (enum), default: `Prefer`
* `trustServerCertificate`: type: boolean, default: `true`

```csharp
using UriCredentialParser;
using UriCredentialParser.Enums;

// ...

var details = CredentialsParser.Parse("postgres://someuser:somepassword@somehost:381/somedatabase");
var connString = details.ToNpgsqlConnectionString(); 
// Result: User ID=someuser;Password=somepassword;Server=somehost;Port=381;Database=somedatabase;Pooling=true;SSL Mode=Prefer;Trust Server Certificate=true

```

This library also allows for generating a MongoDB-compatible connection string alongside the extracted database name:

```csharp
using UriCredentialParser;

// ...

var details = CredentialsParser.Parse("mongodb://user:password@host:port/database-name?otheroptions");
var (dbUrl, dbName) = details.ToMongoConnectionSplit(); 

// dbUrl: mongodb://user:password@host:port?otheroptions
// dbName: database-name

```

## Contributing

Feel free to make requests and to open pull requests with fixes and updates.