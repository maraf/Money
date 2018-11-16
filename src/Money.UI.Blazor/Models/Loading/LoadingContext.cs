using Neptuo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Models.Loading
{
    public class LoadingContext
    {
        protected int OperationCount { get; set; }

        public event Action<LoadingContext> LoadingChanged;

        public bool IsLoading
        {
            get => OperationCount != 0;
        }

        public System.IDisposable Start()
            => new Disposable(this);

        private class Disposable : DisposableBase
        {
            private readonly LoadingContext context;

            public Disposable(LoadingContext context)
            {
                Ensure.NotNull(context, "context");
                this.context = context;

                context.OperationCount++;
                if (context.OperationCount == 1)
                    context.LoadingChanged?.Invoke(context);
            }

            protected override void DisposeManagedResources()
            {
                base.DisposeManagedResources();

                context.OperationCount--;
                if (context.OperationCount == 0)
                    context.LoadingChanged?.Invoke(context);
            }
        }
    }
}
