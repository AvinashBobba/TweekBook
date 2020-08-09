using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TweekBook.Cache;
using TweekBook.Services;

namespace TweekBook.Installers
{
    public class CacheInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var reddisCacheSettings = new RedisCacheSettings();
            configuration.GetSection(nameof(RedisCacheSettings)).Bind(reddisCacheSettings);
            services.AddSingleton(reddisCacheSettings);

            if (!reddisCacheSettings.Enabled)
            {
                return;
            }
            services.AddStackExchangeRedisCache(op => op.Configuration = reddisCacheSettings.ConnectionString);
            services.AddSingleton<IResponseCacheService,ResponseCacheService>();
        }
    }
}
