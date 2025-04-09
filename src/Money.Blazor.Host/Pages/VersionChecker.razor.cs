using Microsoft.AspNetCore.Components;
using Money.Services;
using Neptuo;
using Neptuo.Exceptions.Handlers;
using Neptuo.Logging;
using System;
using System.Threading.Tasks;

namespace Money.Pages;

public partial class VersionChecker(
    ExceptionHandlerBuilder ExceptionHandlerBuilder,
    Navigator Navigator,
    ILog<VersionChecker> Log
) : 
    IExceptionHandler<NotSupportedApiVersionException>
{
    [Parameter]
    public RenderFragment ChildContent { get; set; }

    protected Version ApiVersion { get; set; }
    protected Version ClientVersion { get; set; }

    protected string NewIssueTitle { get; set; }

    protected override void OnInitialized()
    {
        Log.Debug("Initialize VersionSupport");

        base.OnInitialized();

        ExceptionHandlerBuilder.Handler<NotSupportedApiVersionException>(this);
        ClientVersion = typeof(VersionChecker).Assembly.GetName().Version;
        NewIssueTitle = $"Client '{Converts.To<Version, string>(ClientVersion)}' is too old";
    }

    protected Task ReloadAsync() 
        => Navigator.ReloadAsync();

    void IExceptionHandler<NotSupportedApiVersionException>.Handle(NotSupportedApiVersionException exception)
    {
        Log.Debug($"Not support version exception raised with API version '{exception.ApiVersion}'.");

        if (ApiVersion != exception.ApiVersion)
        {
            ApiVersion = exception.ApiVersion;
            StateHasChanged();
        }
    }
}
