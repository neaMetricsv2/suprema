using Suprema.Core.Models;

namespace Suprema.Web.ViewModels;

public sealed class ContentPageViewModel
{
    public required ContentPage Page { get; init; }
    public string? RenderedHtml { get; init; }
}
