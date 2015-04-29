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
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Moq;

namespace MobileApped.Core.Persistence.Testing.DataContexts
{
    [ExcludeFromCodeCoverage]
    public static class MockDbContextExtensions
    {
       public static IDbSet<TSetType> SetUpData<TContextType, TSetType>(
           this Mock<TContextType> context,
            IEnumerable<TSetType> entities,
            Expression<Func<TContextType, IDbSet<TSetType>>> expression = null)
            where TSetType : Entity, new()
            where TContextType : DbContext
        {
            IQueryable queryable = entities.AsQueryable();
            Mock<DbSet<TSetType>> entitySet = new Mock<DbSet<TSetType>>();

            Mock<IDbSet<TSetType>> idbset = entitySet.As<IDbSet<TSetType>>();
            idbset.Setup(d => d.GetEnumerator()).Returns(entities.GetEnumerator());
            idbset.SetupGet(d => d.Provider).Returns(queryable.Provider);
            idbset.SetupGet(d => d.ElementType).Returns(queryable.ElementType);
            idbset.SetupGet(d => d.Expression).Returns(queryable.Expression);

            context.Setup(d => d.Set<TSetType>().AsNoTracking()).Returns(entitySet.Object);

            if (expression != null)
            {
                context.SetupGet(expression).Returns(entitySet.Object);
            }

            return idbset.Object;
        }
    }
}