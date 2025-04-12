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

    public async Task<bool> NextAsync()
    {
        if (IsLoading || !HasNextPage)
            return false;

        using (loading.Start())
        {
            if (!HasNextPage)
                return false;

            CurrentPageIndex++;
            var status = await loadPageAsync();
            ProcessStatus(status);
        }

        return true;
    }

    public async Task<bool> PrevAsync()
    {
        if (IsLoading || !HasNextPage)
            return false;

        using (loading.Start())
        {
            if (CurrentPageIndex == 0)
                return false;

            CurrentPageIndex--;
            var status = await loadPageAsync();
            ProcessStatus(status);
        }

        return true;
    }

    public async Task<PagingLoadStatus> LoadAsync(int index)
    {
        if (IsLoading)
            return PagingLoadStatus.HasNextPage;

        using (loading.Start())
        {
            CurrentPageIndex = index;
            var status = await loadPageAsync();
            ProcessStatus(status);
            return status;
        }
    }
}
