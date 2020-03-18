using Microsoft.AspNetCore.Components;
using Money.Services;
using Neptuo.Exceptions.Handlers;
using Neptuo.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages
{
    public partial class VersionSupport : IExceptionHandler<NotSupportedApiVersionException>
    {
        [Inject]
        public ExceptionHandlerBuilder ExceptionHandlerBuilder { get; set; }

        [Inject]
        public Navigator Navigator { get; set; }

        [Inject]
        public ILog<VersionSupport> Log { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected Version ApiVersion { get; set; }
        protected Version ClientVersion { get; set; }

        protected override void OnInitialized()
        {
            Log.Debug("Initialize VersionSupport");

            base.OnInitialized();

            ExceptionHandlerBuilder.Handler<NotSupportedApiVersionException>(this);
            ClientVersion = typeof(VersionSupport).Assembly.GetName().Version;
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
}
