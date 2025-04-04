using Money.Models.Loading;
using Neptuo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Components;

public class PagingContext(Func<Task<PagingLoadStatus>> loadPageAsync, LoadingContext loading)
{
    public int CurrentPageIndex { get; private set; }
    public bool HasNextPage { get; private set; } = true;
    public bool IsLoading => loading.IsLoading;

    public Func<Task<PagingLoadStatus>> LoadPageAsync { get; } = loadPageAsync;

    private void ProcessStatus(PagingLoadStatus status)
    {
        switch (status)
        {
            case PagingLoadStatus.HasNextPage:
                HasNextPage = true;
                break;

            case PagingLoadStatus.LastPage:
                HasNextPage = false;
                break;

            case PagingLoadStatus.EmptyPage:
                HasNextPage = false;
                if (CurrentPageIndex > 0)
                    CurrentPageIndex--;
                break;

            default:
                throw Ensure.Exception.NotSupported(status.ToString());
        }
    }

    public async Task NextAsync()
    {
        using (loading.Start())
        {
            if (!HasNextPage)
                return;

            CurrentPageIndex++;
            var status = await LoadPageAsync();
            ProcessStatus(status);
        }
    }

    public async Task PrevAsync()
    {
        using (loading.Start())
        {
            if (CurrentPageIndex == 0)
                return;

            CurrentPageIndex--;
            var status = await LoadPageAsync();
            ProcessStatus(status);
        }
    }

    public async Task<PagingLoadStatus> LoadAsync(int index)
    {
        using (loading.Start())
        {
            CurrentPageIndex = index;
            var status = await LoadPageAsync();
            ProcessStatus(status);
            return status;
        }
    }
}
