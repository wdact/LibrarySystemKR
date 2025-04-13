using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryClientKR.Helpers
{
    internal static class IQueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type);

            return (IQueryable<T>)method.Invoke(null, new object[] { source, lambda });
        }

        public static IQueryable<T> OrderByDescendingDynamic<T>(this IQueryable<T> source, string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var lambda = Expression.Lambda(property, parameter);

            var method = typeof(Queryable).GetMethods()
                .First(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type);

            return (IQueryable<T>)method.Invoke(null, new object[] { source, lambda });
        }
    }
}
