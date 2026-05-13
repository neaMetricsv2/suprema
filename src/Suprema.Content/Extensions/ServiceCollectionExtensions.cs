using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Suprema.Core.Abstractions;
using Suprema.Content.Services;

namespace Suprema.Content.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSupremaContent(this IServiceCollection services, string contentRoot)
    {
        services.AddSingleton<IContentService>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<ContentService>>();
            return new ContentService(contentRoot, logger);
        });
        return services;
    }
}
