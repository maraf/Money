using Microsoft.AspNetCore.Components;
using Money.Services;
using Neptuo.Logging;
using System;
using System.Threading.Tasks;

namespace Money.Pages;

public partial class ApiHubConnectionChecker(
    ServerConnectionState ServerConnection,
    ApiHubService ApiHub,
    Navigator Navigator,
    ILog<ApiHubConnectionChecker> Log
) : 
    IDisposable
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        Log.Debug("Bind on changed.");
        ServerConnection.Changed += OnStateChanged;
        ApiHub.Changed += OnApiHubStateChanged;
    }

    private void OnApiHubStateChanged(ApiHubStatus status, Exception e)
    {
        Log.Debug("OnApiHubStateChanged, rerendering.");
        StateHasChanged();
    }

    private void OnStateChanged(bool isAvailable)
    {
        Log.Debug("OnStateChanged, rerendering.");
        StateHasChanged();
    }

    protected Task ReloadAsync()
        => Navigator.ReloadAsync();

    public void Dispose()
    {
        ServerConnection.Changed -= OnStateChanged;
        ApiHub.Changed -= OnApiHubStateChanged;
    }

    protected Task ReconnectAsyc()
        => ApiHub.StartAsync();
}
