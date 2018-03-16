using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MobileApped.Core.Persistence;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MobileAppedServiceCollectionExtensions
    {
        public static IServiceCollection AddInMemoryDataContext<TDbContext>(
            this IServiceCollection services,
            IConfiguration configuration,
            string databaseName)
                where TDbContext : DbContext
        {
            services.AddDbContext<TDbContext>(d =>
                d.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .UseInMemoryDatabase(databaseName), 
                contextLifetime: ServiceLifetime.Transient, 
                optionsLifetime: ServiceLifetime.Singleton)
                .AddEntityFrameworkInMemoryDatabase();
            
            services.AddSingleton<IDataContext<TDbContext>, DataContext<TDbContext>>();
            return services;
        }
    }
}
