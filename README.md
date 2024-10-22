# Quiz Application:: Back end API:
Author: June (Xuan trinh)
Intention: Provide logic for front end: NextJs application
Version 1:

## Tech stack:
**Language**: C#

**Framework**: Web API dotnet.

**Authentication**:

	custom JWT: _(could easily swap out for 3rd party IDaaS (e.g: Auth0))_

	Key-type: _Symmetric <- simplicity, speed of development, Performance._

	Hashing: _use BCrypt:: <utilize their Blowfish algo> since our app will have small scope a default cost factor (10 or 1024 iterations) is sufficient_

	Secret: _generate using OpenSSL_

	Currently: Not using any security model (not Role or Policy base yet @23/10/24)

**Security**: Design follow zero trust architecture.

**API Documentation**: SwagBuckle

**Data Gateway**: Entity Framework Core(ORM) allow data access to MSSQL server. (No repository pattern yet)

**Data Layer**: MSSQL DBMS

**Dependancies**: consult the project solution

**Test**: Not intergarate

## Requirement:
[.NET SDK (Version 6 or later)](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) 
[MsSQL server or any other SQL DBMS that support by EF core](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Setting
@appsetings.json
```
"ConnectionStrings": {
    "ISpaceDbConnectionString": "Server=\"Ip address of your db server\"\\MSSQL1SERVER;Database=\"Your DB alias\";Trusted_Connection=True;TrustServerCertificate=True"
  },

"JwtSettings": {
    "Issuer": "https://ip:port (or Domain)/*",
    "Audience": "https://ip:port (or Domain)/*",
    "ExpiryMinutes": var(how long the expiration)
  },
```
@also need user secret @JwtSetting.

## Initiate:
@Console:: install all the missing dependancy
```
dotnet restore
```
@Nuget Console:: run the migration code.
```
Update-database
```

## Run
the application is run on Https://localhost:5001 (only via with https)


# TODO:
[] change the serialized JSON [From body] into JSON [From body]

[] Add Api documentation using Dotnet Core.Mvc to enable run time interfere

[] Use the BusinessToPresentationDto in all controller

[x] CRUD

[x] Edit question

[x] Create question

[x] create collection

[x] GetAllCollections

[x] delete question
	
[x] Validator

[x] Quiz

[] Implement more auto mapper 

[] apply the repository pattern

[] make the user Name unique

[] Implement exception handling accross application especially in the gate way between controller and service

[] use OpenSSl to create new secrete

[] @Production@Stage:: migrate secret to safe vault (currently store in user-secret)

[] implement the login request count to detect mal-behaviour

[] investigate the Reactor pattern for low latency game player machanism between the server and client