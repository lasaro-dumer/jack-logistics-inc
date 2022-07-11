## Requirements

- Dotnet 5 SDK installed locally
- EF Tools installed (run `dotnet tool install --global dotnet-ef`)
- A Postgree database ready for use

## Testing the API

To run the tests cases for the API, run this command:

```
dotnet test .\src\JackLogisticsInc.API.Tests\JackLogisticsInc.API.Tests.csproj
```

The testing uses an in memory database, so no real database is needed and it can be executed even before the next configurations.

### Configuring the local environment

Add the following configuration (changing the pertinent details) to the file `src\JackLogisticsInc.API\appsettings.Development.json` (or open the project user secrets, on (VSCode)[https://marketplace.visualstudio.com/items?itemName=adrianwilczynski.user-secrets] or (Visual Studio)[https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?tabs=windows#set-a-secret]):

```
    "ConnectionStrings": {
        "DefaultConnection": "User ID=dev;Password=123456789;Host=localhost;Port=5445;Database=JackLogisticsInc;"
    }
```

After the connection string is configured, run:

```
dotnet ef database update --project .\src\JackLogisticsInc.API\JackLogisticsInc.API.csproj
```

## Running the application

On this README folder (from now on referenced as `root`), run:

```
dotnet run --project .\src\JackLogisticsInc.API\JackLogisticsInc.API.csproj
```

Open another terminal (on the root folder) and run:

```
yarn --cwd .\src\jack-logi-ui\ install
```

After the installation completes, then run:

```
yarn --cwd .\src\jack-logi-ui\ start
```
