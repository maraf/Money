using Microsoft.AspNetCore.Components;
using Neptuo.Logging;
using System;
using System.Collections.Generic;

namespace Money.Components;

partial class Placeholder(ILog<Placeholder> Log)
{
    private static readonly PlaceholderContext activeContext = new PlaceholderContext("placeholder", true);

    [CascadingParameter]
    public PlaceholderContainer Container { get; set; }

    [Parameter]
    public RenderFragment ChildContent { get; set; }

    [Parameter]
    public RenderFragment<PlaceholderContext> PlaceholderContent { get; set; }

    [Parameter]
    public int WordMinCount { get; set; } = 1;

    [Parameter]
    public int WordMaxCount { get; set; } = 1;

    [Parameter]
    public int WordMinLength { get; set; } = 1;

    [Parameter]
    public int WordMaxLength { get; set; } = 9;

    [Parameter]
    public int? TotalMaxLength { get; set; }

    protected List<int> WordLengths { get; set; }

    protected bool IsActive => Container?.IsActive ?? false;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        if (IsActive)
        {
            if (PlaceholderContent == null)
            {
                WordLengths ??= new List<int>();
                WordLengths.Clear();

                int currentTotalLength = 0;
                var wordCount = Random.Shared.Next(WordMinCount, WordMaxCount);
                for (int i = 0; i < wordCount; i++)
                {
                    var currentMinLength = WordMinLength;
                    var currentMaxLength = WordMaxLength;
                    if (i == wordCount - 1)
                    {
                        currentMaxLength = Math.Min((TotalMaxLength ?? Int32.MaxValue) - currentTotalLength, WordMaxLength);
                        currentMinLength = Math.Min(
                            Math.Max((TotalMaxLength ?? 0) - currentTotalLength, WordMinLength),
                            currentMaxLength
                        );
                    }
                    var newLength = Random.Shared.Next(currentMinLength, currentMaxLength);
                    currentTotalLength += newLength;
                    WordLengths.Add(newLength);
                }

                Log.Debug($"Selected placeholders '{String.Join(", ", WordLengths)}' for WordCount={WordMinCount}/{WordMaxCount} and Lengths={WordMinLength}/{WordMaxLength} and TotalLength={TotalMaxLength}");
            }
            else
            {
                Log.Debug($"Selected placeholders skipped due to PlaceholderContent");
            }
        }
        else
        {
            Log.Debug($"Selected placeholders skipped due to inactive");
        }
    }
}