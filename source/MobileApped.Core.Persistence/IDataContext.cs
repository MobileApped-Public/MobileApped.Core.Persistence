using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.EntityFrameworkCore;

namespace MobileApped.Core.Persistence
{
    public interface IDataContext<TDbContext>
        where TDbContext : DbContext
    {
        void Insert<TEntityType>(TEntityType entity)
    where TEntityType : class;

        Task InsertAsync<TEntityType>(TEntityType entity)
            where TEntityType : class;

        void Update<TEntityType>(TEntityType entity)
            where TEntityType : class;

        Task UpdateAsync<TEntityType>(TEntityType entity)
            where TEntityType : class;

        void Delete<TEntityType>(TEntityType entity)
            where TEntityType : class;

        Task DeleteAsync<TEntityType>(TEntityType entity)
            where TEntityType : class;

        void UsingContext(Action<TDbContext> action);

        TResultType UsingContext<TResultType>(Func<TDbContext, TResultType> action);

        void UsingTransaction(
            Action<TransactionScope, TDbContext> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadUncommitted);

        void Query<TEntityType>(
            Action<IQueryable<TEntityType>> query,
            params Expression<Func<TEntityType, object>>[] includes)
            where TEntityType : class;

        TResultType Query<TEntityType, TResultType>(
            Func<IQueryable<TEntityType>, TResultType> query,
            params Expression<Func<TEntityType, object>>[] includes)
            where TEntityType : class;

        IEnumerable<TEntityType> SqlQuery<TEntityType>(string sqlOrSproc, params object[] parameters)
            where TEntityType : class;
    }
}