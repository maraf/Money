#:sdk Aspire.AppHost.Sdk@13.2.3
#:project .\Money.Api\Money.Api.csproj
#:project .\Money.Blazor.Host\Money.Blazor.Host.csproj

var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions 
{
    DashboardApplicationName = "Money AppHost",
    Args = args,
});

var api = builder
    .AddProject<Projects.Money_Api>("api");

// var isWatch = builder.Configuration.GetValue<string>("DOTNET_WATCH") == "1";
var isWatch = true;
if (isWatch)
{
    var uiProjectDirectory = Path.GetDirectoryName(new Projects.Money_Blazor_Host().ProjectPath)!;

    builder
        .AddExecutable("ui", "dotnet", uiProjectDirectory, ["watch", "--non-interactive", "--verbose"])
        .WithEnvironment(context =>
        {
            context.EnvironmentVariables["DOTNET_WATCH_SUPPRESS_LAUNCH_BROWSER"] = "1";
            context.EnvironmentVariables["DOTNET_WATCH_RESTART_ON_RUDE_EDIT"] = "1";
        })
        .WithHttpEndpoint(targetPort: 48613, isProxied: false)
        .WithReference(api);
}
else
{
    builder
        .AddProject<Projects.Money_Blazor_Host>("ui")
        .WithReference(api);
}

builder.Build().Run();
