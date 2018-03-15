using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MobileApped.Core.Persistence
{
    public class DataContext<TDbContext> : IDataContext<TDbContext>
            where TDbContext : DbContext
    {
        private IServiceProvider serviceProvider;

        public DataContext(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        protected virtual TDbContext CreateContext()
        {
            TDbContext context = serviceProvider.GetService<TDbContext>();
            context.ChangeTracker.AutoDetectChangesEnabled = false;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            return context;
        }

        public void Insert<TEntityType>(TEntityType entity)
            where TEntityType : class
        {
            Save(entity, EntityState.Added);
        }

        public async Task InsertAsync<TEntityType>(TEntityType entity)
            where TEntityType : class
        {
            await SaveAsync(entity, EntityState.Added);
        }

        public void Update<TEntityType>(TEntityType entity)
            where TEntityType : class
        {
            Save(entity, EntityState.Modified);
        }

        public async Task UpdateAsync<TEntityType>(TEntityType entity)
            where TEntityType : class
        {
            await SaveAsync(entity, EntityState.Modified);
        }

        public void Delete<TEntityType>(TEntityType entity)
            where TEntityType : class
        {
            Save(entity, EntityState.Deleted);
        }

        public async Task DeleteAsync<TEntityType>(TEntityType entity)
            where TEntityType : class
        {
            await SaveAsync(entity, EntityState.Deleted);
        }

        public void Save<TEntityType>(TEntityType entity, EntityState state)
            where TEntityType : class
        {
            using (TDbContext context = CreateContext())
            {
                context.Entry(entity).State = state;
                context.SaveChanges();
            }
        }

        public async Task SaveAsync<TEntityType>(TEntityType entity, EntityState state)
            where TEntityType : class
        {
            using (TDbContext context = CreateContext())
            {
                context.Entry(entity).State = state;
                await context.SaveChangesAsync();
            }
        }

        public void UsingContext(Action<TDbContext> action)
        {
            using (var context = CreateContext())
            {
                action(context);
            }
        }

        public TResultType UsingContext<TResultType>(Func<TDbContext, TResultType> action)
        {
            using (var context = CreateContext())
            {
                return action(context);
            }
        }

        public void UsingTransaction(
            Action<TransactionScope, TDbContext> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted)
        {
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = isolationLevel;
            using (TransactionScope transactionScope =
                new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                using (var context = CreateContext())
                {
                    action(transactionScope, context);
                    transactionScope.Complete();
                }
            }
        }

        public void Query<TEntityType>(
            Action<IQueryable<TEntityType>> query,
            params Expression<Func<TEntityType, object>>[] includes)
            where TEntityType : class
        {
            using (TDbContext context = CreateContext())
            {
                IQueryable<TEntityType> set = context.Set<TEntityType>();
                foreach (var expression in includes)
                {
                    set = set.Include(expression);
                }
                query(set);
            }
        }

        public TResultType Query<TEntityType, TResultType>(
            Func<IQueryable<TEntityType>, TResultType> query,
            params Expression<Func<TEntityType, object>>[] includes)
            where TEntityType : class
        {
            using (TDbContext context = CreateContext())
            {
            IQueryable<TEntityType> set = context.Set<TEntityType>();
                foreach (var expression in includes)
                {
                    set = set.Include(expression);
                }
                return query(set);
            }
        }

        public IEnumerable<TEntityType> SqlQuery<TEntityType>(string sqlOrSproc, params object[] parameters)
            where TEntityType : class
        {
            using (TDbContext context = CreateContext())
            {
                return context.Set<TEntityType>().FromSql(sqlOrSproc, parameters).ToList();
            }
        }
    }
}
