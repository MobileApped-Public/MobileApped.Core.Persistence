using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MobileApped.Core.Persistence;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MobileAppedServiceCollectionExtensions
    {
        public static IServiceCollection AddSqlServerDataContext<TDbContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string connectionName,
            int retryCount = 0)
                where TDbContext : DbContext
        {
            string connectionString = configuration.GetConnectionString(connectionName);

            services.AddDbContext<TDbContext>(d =>
                d.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseSqlServer(connectionString, options =>
                {
                    if (retryCount > 0)
                    {
                        options.EnableRetryOnFailure(retryCount);
                    }
                }),
                contextLifetime: ServiceLifetime.Transient,
                optionsLifetime: ServiceLifetime.Singleton);

            services.AddSingleton<IDataContext<TDbContext>, DataContext<TDbContext>>();
            return services;
        }
    }
}
