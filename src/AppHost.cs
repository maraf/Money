#:sdk Aspire.AppHost.Sdk@13.2.4
#:project ./Money.Api/Money.Api.csproj
#:project ./Money.Blazor.Host/Money.Blazor.Host.csproj

var builder = DistributedApplication.CreateBuilder(new DistributedApplicationOptions 
{
    DashboardApplicationName = "Money AppHost",
    Args = args,
});

var api = builder
    .AddProject<Projects.Money_Api>("api");

builder
    .AddProject<Projects.Money_Blazor_Host>("ui")
    .WithReference(api);

builder.Build().Run();
