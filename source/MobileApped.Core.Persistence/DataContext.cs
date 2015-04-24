#region COPYRIGHT
/* 
   Copyright 2015 MobileApped.com

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

namespace MobileApped.Core.Persistence
{
    /// <summary>
    /// Provides a reusable method for accessing and manipulating database entities
    /// </summary>
    /// <typeparam name="TDataProvider">Type of DbContext</typeparam>
    public class DataContext<TDataProvider> : IDataContext<TDataProvider>
         where TDataProvider : DbContext, new()
    {
        /// <inheritdoc />
        public ICollection<TEntityType> ExecuteSetQuery<TEntityType>(string query, params object[] args)
            where TEntityType : Entity
        {
            using (DbContext context = CreateContext())
            {
                return context.Set<TEntityType>().SqlQuery(query, args).AsNoTracking().ToList();
            }
        }

        /// <inheritdoc />
        public ICollection<TResultType> ExecuteCustomQuery<TResultType>(string query, params object[] args)
        {
            using (DbContext context = CreateContext())
            {
                return context.Database.SqlQuery<TResultType>(query, args).ToList();
            }
        }

        /// <inheritdoc />
        public void ExecuteAction<TEntityType>(Action<DbQuery<TEntityType>> action)
            where TEntityType : Entity
        {
            using (DbContext context = CreateContext())
            {
                action(context.Set<TEntityType>().AsNoTracking());
            }
        }

        /// <inheritdoc />
        public TResultType SelectFromSet<TEntityType, TResultType>(Func<DbQuery<TEntityType>, TResultType> action)
                where TEntityType : Entity
        {
            using (DbContext context = CreateContext())
            {
                return action(context.Set<TEntityType>().AsNoTracking());
            }
        }

        /// <inheritdoc />
        public void UsingContext(Action<TDataProvider> action)
        {
            using (DbContext context = CreateContext())
            {
                action(context as TDataProvider);
            }
        }

        /// <inheritdoc />
        public TResultType UsingContext<TResultType>(Func<TDataProvider, TResultType> action)
        {
            using (DbContext context = CreateContext())
            {
                return action(context as TDataProvider);
            }
        }

        /// <inheritdoc />
        public TEntityType FirstOrDefault<TEntityType>(
            Expression<Func<TEntityType, bool>> predicate, 
            params Expression<Func<TEntityType, object>>[] includes)
            where TEntityType : Entity
        {
            using (DbContext context = CreateContext())
            {
                DbQuery<TEntityType> query = CreateSetQuery<TEntityType>(context, includes);
                return query.FirstOrDefault(predicate);
            }
        }

        /// <inheritdoc />
        public TEntityType SingleOrDefault<TEntityType>(
            Expression<Func<TEntityType, bool>> predicate, 
            params Expression<Func<TEntityType, object>>[] includes)
            where TEntityType : Entity
        {
            using (DbContext context = CreateContext())
            {
                DbQuery<TEntityType> query = CreateSetQuery<TEntityType>(context, includes);
                return query.SingleOrDefault(predicate);
            }
        }

        /// <inheritdoc />
        public ICollection<TEntityType> Where<TEntityType>(
            Expression<Func<TEntityType, bool>> predicate, 
            params Expression<Func<TEntityType, object>>[] includes)
            where TEntityType : Entity
        {
            using (DbContext context = CreateContext())
            {
                DbQuery<TEntityType> query = CreateSetQuery<TEntityType>(context, includes);
                return query.Where(predicate).ToList();
            }
        }

        /// <inheritdoc />
        public void Insert<TEntityType>(TEntityType entity)
            where TEntityType : Entity
        {
            using (DbContext context = CreateContext())
            {
                ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
                ObjectSet<TEntityType> objectSet = objectContext.CreateObjectSet<TEntityType>();
                objectSet.MergeOption = MergeOption.NoTracking;

                objectSet.AddObject(entity);
                objectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Added);

                context.SaveChanges();
            }
        }

        /// <inheritdoc />
        public void Insert<TEntityType>(IEnumerable<TEntityType> entities)
            where TEntityType : Entity
        {
            using (DbContext context = CreateContext())
            {
                ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
                ObjectSet<TEntityType> objectSet = objectContext.CreateObjectSet<TEntityType>();
                objectSet.MergeOption = MergeOption.NoTracking;

                foreach (TEntityType entity in entities)
                {
                    objectSet.AddObject(entity);
                    objectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Added);
                }

                context.SaveChanges();
            }
        }

        /// <inheritdoc />
        public void Update<TEntityType>(TEntityType entity)
            where TEntityType : Entity
        {
            using (DbContext context = CreateContext())
            {
                ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
                ObjectSet<TEntityType> objectSet = objectContext.CreateObjectSet<TEntityType>();
                objectSet.MergeOption = MergeOption.NoTracking;

                objectSet.Attach(entity);
                objectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);

                context.SaveChanges();
            }
        }

        /// <inheritdoc />
        public void Update<TEntityType>(IEnumerable<TEntityType> entities)
            where TEntityType : Entity
        {
            using (DbContext context = CreateContext())
            {
                ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
                ObjectSet<TEntityType> objectSet = objectContext.CreateObjectSet<TEntityType>();
                objectSet.MergeOption = MergeOption.NoTracking;

                foreach (TEntityType dataObject in entities)
                {
                    objectSet.Attach(dataObject);
                    objectContext.ObjectStateManager.ChangeObjectState(dataObject, EntityState.Modified);
                }

                context.SaveChanges();
            }
        }

        /// <inheritdoc />
        public void Delete<TEntityType>(TEntityType entity)
            where TEntityType : Entity
        {
            using (DbContext context = CreateContext())
            {
                ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
                ObjectSet<TEntityType> objectSet = objectContext.CreateObjectSet<TEntityType>();
                objectSet.MergeOption = MergeOption.NoTracking;

                objectSet.Attach(entity);
                objectContext.ObjectStateManager.ChangeObjectState(entity, EntityState.Deleted);

                context.SaveChanges();
            }
        }

        /// <inheritdoc />
        public void Delete<TEntityType>(IEnumerable<TEntityType> entities)
            where TEntityType : Entity
        {
            using (DbContext context = CreateContext())
            {
                ObjectContext objectContext = ((IObjectContextAdapter)context).ObjectContext;
                ObjectSet<TEntityType> objectSet = objectContext.CreateObjectSet<TEntityType>();
                objectSet.MergeOption = MergeOption.NoTracking;

                foreach (TEntityType dataObject in entities)
                {
                    objectSet.Attach(dataObject);
                    objectContext.ObjectStateManager.ChangeObjectState(dataObject, EntityState.Deleted);
                }

                context.SaveChanges();
            }
        }

        public void AsTransaction(Action action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            TransactionOptions transactionOptions = new TransactionOptions();
            transactionOptions.IsolationLevel = isolationLevel;

            using (TransactionScope transactionScope = new TransactionScope(TransactionScopeOption.Required, transactionOptions))
            {
                action();
                transactionScope.Complete();
            }
        }

        protected virtual DbContext CreateContext()
        {
            DbContext context = new TDataProvider();
            context.Configuration.AutoDetectChangesEnabled = false;

            return context;
        }
        
        protected DbQuery<TEntityType> CreateSetQuery<TEntityType>(
            DbContext context, 
            params Expression<Func<TEntityType, object>>[] includes)
            where TEntityType : class
        {
            DbQuery<TEntityType> query = context.Set<TEntityType>().AsNoTracking();
            foreach (var include in includes)
            {
                query = (DbQuery<TEntityType>)query.Include(include);
            }
            return query;
        }
    }
}