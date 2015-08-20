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
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Transactions;

namespace MobileApped.Core.Persistence
{
    public interface IDataContext<TDataProvider>
            where TDataProvider : DbContext
    {
        /// <summary>
        /// Executes a custom sql script against the specified entity dataset.
        /// <para>
        /// Result of the query must match the specified entity.
        /// </para>
        /// <example>
        ///     <para>
        ///         StoredProcedure: GetEmployeeByDepartment
        ///     </para>
        ///     <para>
        ///         SQL Script: SELECT * FROM Employee
        ///     </para>
        /// </example>
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity</typeparam>
        /// <param name="query">SQL Query</param>
        /// <param name="args">Script parameters</param>
        /// <returns>Enumeration of the specified entity type</returns>
        ICollection<TEntityType> ExecuteSetQuery<TEntityType>(string query, params object[] args)
           where TEntityType : Entity;

        /// <summary>
        /// Executes an awaitable custom sql script against the specified entity dataset.
        /// <para>
        /// Result of the query must match the specified entity.
        /// </para>
        /// <example>
        ///     <para>
        ///         StoredProcedure: GetEmployeeByDepartment
        ///     </para>
        ///     <para>
        ///         SQL Script: SELECT * FROM Employee
        ///     </para>
        /// </example>
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity</typeparam>
        /// <param name="query">SQL Query</param>
        /// <param name="args">Script parameters</param>
        /// <returns>Enumeration of the specified entity type</returns>
        Task<ICollection<TEntityType>> ExecuteSetQueryAsync<TEntityType>(string query, params object[] args)
           where TEntityType : Entity;

        /// <summary>
        /// Executes a custom sql script against the specified database
        /// <para>
        /// Result of the query can be a custom class or primitive type as long as properties map
        /// </para>
        /// <example>
        ///     <para>
        ///         SELECT GETDATE()
        ///     </para>
        /// </example>
        /// </summary>
        /// <typeparam name="TResultType">Type of data that script returns</typeparam>
        /// <param name="query">SQL query to execute</param>
        /// <param name="args">Script parameters</param>
        /// <returns>Enumeration of the specified type</returns>
        ICollection<TResultType> ExecuteCustomQuery<TResultType>(string query, params object[] args);

        void UsingContext(Action<TDataProvider> action);

        TResultType UsingContext<TResultType>(Func<TDataProvider, TResultType> action);

        TEntityType FirstOrDefault<TEntityType>(Expression<Func<TEntityType, bool>> predicate, params Expression<Func<TEntityType, object>>[] includes)
            where TEntityType : Entity;

        TEntityType SingleOrDefault<TEntityType>(Expression<Func<TEntityType, bool>> predicate, params Expression<Func<TEntityType, object>>[] includes)
            where TEntityType : Entity;

        ICollection<TEntityType> Where<TEntityType>(Expression<Func<TEntityType, bool>> predicate, params Expression<Func<TEntityType, object>>[] includes)
            where TEntityType : Entity;

        void Insert<TEntityType>(TEntityType entity)
            where TEntityType : Entity;

        void Insert<TEntityType>(IEnumerable<TEntityType> entities)
            where TEntityType : Entity;

        void Update<TEntityType>(TEntityType entity)
            where TEntityType : Entity;

        void Update<TEntityType>(IEnumerable<TEntityType> entities)
            where TEntityType : Entity;

        void Delete<TEntityType>(TEntityType entity)
            where TEntityType : Entity;

        void Delete<TEntityType>(IEnumerable<TEntityType> entities)
            where TEntityType : Entity;

        /// <summary>
        /// Creates and calls complete after executing the specified action
        /// </summary>
        /// <param name="action">Action to invoke</param>
        /// <param name="isolationLevel">Transaction isolation level</param>
        void AsTransaction(Action action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        /// <summary>
        /// Allows execution of custom linq query against the specified data set
        /// </summary>
        /// <typeparam name="TEntityType">Type of entity</typeparam>
        /// <param name="action">Action to invoke</param>
        [Obsolete("Use UsingContext")]
        void ExecuteAction<TEntityType>(Action<DbQuery<TEntityType>> action)
            where TEntityType : Entity;

        [Obsolete("Use UsingContext")]
        TResultType SelectFromSet<TEntityType, TResultType>(Func<DbQuery<TEntityType>, TResultType> action)
            where TEntityType : Entity;
    }
}