using Microsoft.AspNetCore.Components;
using Money.Services;
using System;

namespace Money.Pages;

public partial class ApiHubConnectionStatus(IApiHubState State) : IDisposable
{
    [Parameter]
    public bool IsTextVisible { get; set; } = true;

    protected string CssClass { get; set; }
    protected string Message { get; set; }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        State.Changed += OnStateChanged;
        UpdateState();
    }

    protected void UpdateState()
    {
        switch (State.Status)
        {
            case ApiHubStatus.Disconnected:
                Message = "Disconnected";
                CssClass = "text-danger";
                break;
            case ApiHubStatus.Connecting:
                Message = "Connecting...";
                CssClass = "text-warning";
                break;
            case ApiHubStatus.Connected:
                Message = "Connected";
                CssClass = "text-success";
                break;
            default:
                break;
        }
    }

    private void OnStateChanged(ApiHubStatus status, Exception e)
    {
        UpdateState();
        StateHasChanged();
    }

    public void Dispose()
        => State.Changed -= OnStateChanged;
}
