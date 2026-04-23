using DotnetCleanArch.Application.Abstractions.Caching;
using DotnetCleanArch.Application.Abstractions.Data;
using DotnetCleanArch.Infrastructure.Caching;
using DotnetCleanArch.Infrastructure.Persistence;
using DotnetCleanArch.Infrastructure.Persistence.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DotnetCleanArch.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddPersistence(configuration);
        services.AddCaching(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<PostgresOptions>()
            .Bind(configuration.GetSection(PostgresOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<UpdateAuditableEntitiesInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            var postgres = sp.GetRequiredService<IOptions<PostgresOptions>>().Value;
            var interceptor = sp.GetRequiredService<UpdateAuditableEntitiesInterceptor>();

            options
                .UseNpgsql(postgres.BuildConnectionString())
                .AddInterceptors(interceptor);
        });

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static IServiceCollection AddCaching(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddOptions<RedisOptions>()
            .Bind(configuration.GetSection(RedisOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddStackExchangeRedisCache(_ => { });

        services
            .AddOptions<Microsoft.Extensions.Caching.StackExchangeRedis.RedisCacheOptions>()
            .Configure<IOptions<RedisOptions>>((cacheOptions, redisOptions) =>
            {
                cacheOptions.ConfigurationOptions = redisOptions.Value.ToConfigurationOptions();
                cacheOptions.InstanceName = redisOptions.Value.InstanceName;
            });

        services.AddScoped<ICacheService, CacheService>();

        return services;
    }
}
