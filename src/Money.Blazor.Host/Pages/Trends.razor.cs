using Microsoft.AspNetCore.Components;
using Money.Models;
using Money.Models.Queries;
using Money.Services;
using Neptuo;
using Neptuo.Models.Keys;
using Neptuo.Queries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Money.Pages;

public partial class Trends(IQueryDispatcher Queries, Navigator Navigator)
{
    private static readonly List<CategoryModel> MockCategories = Enumerable.Range(0, 6)
        .Select(_ => new CategoryModel(KeyFactory.Create(typeof(Category)), string.Empty, string.Empty, Color.FromArgb(255, 233, 236, 239), string.Empty))
        .ToList();

    protected List<CategoryModel> Categories { get; set; }

    protected async override Task OnInitializedAsync()
    {
        await base.OnInitializedAsync();

        Categories = await Queries.QueryAsync(new ListAllCategory());
    }
}
