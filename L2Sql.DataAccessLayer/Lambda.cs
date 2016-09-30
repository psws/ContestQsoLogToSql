using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace L2Sql.DataAccessLayer
{
    public class Lambda
    {
    }


    public class Comparer<T> : IEqualityComparer<T>
    {
        private readonly Func<T, T, bool> _expression;

        public Comparer(Func<T, T, bool> lambda)
        {
            _expression = lambda;
        }

        public bool Equals(T x, T y)
        {
            return _expression(x, y);
        }

        public int GetHashCode(T obj)
        {
            /*  If you just return 0 for the hash the Equals comparer will kick in.
             * The underlying evaluation checks the hash and then short circuits the evaluation if it is false.
             * Otherwise, it checks the Equals. If you force the hash to be true (by assuming 0 for both objects),
             * you will always fall through to the Equals check which is what we are always going for.
             * 
             */
            return 0;
        }
    }
  
    public static class EnumerableExtension
    { 
        public static IEnumerable<T> Except<T>(this IEnumerable<T> listA, IEnumerable<T> listB, Func<T, T, bool> lambda)
        {             
            return listA.Except(listB, new Comparer<T>(lambda));
        }         
        
        public static IEnumerable<T> Intersect<T>(this IEnumerable<T> listA, IEnumerable<T> listB, Func<T, T, bool> lambda)
        {             
            return listA.Intersect(listB, new Comparer<T>(lambda));
        }


        //http://stackoverflow.com/questions/1210295/how-can-i-add-an-item-to-a-ienumerablet-collection
        //public static IEnumerable Append(this IEnumerable first, params object[] second)
        //{
        //    return first.OfType<object>().Concat(second);
        //}
        public static IEnumerable<T> Append<T>(this IEnumerable<T> first, params T[] second)
        {
            return first.Concat(second);
        }
        //public static IEnumerable Prepend(this IEnumerable first, params object[] second)
        //{
        //    return second.Concat(first.OfType<object>());
        //}
        public static IEnumerable<T> Prepend<T>(this IEnumerable<T> first, params T[] second)
        {
            return second.Concat(first);
        }


        //http://stackoverflow.com/questions/3930510/simple-way-to-update-ienumerable-objects-using-linq
        public static void Update<TSource>(this IEnumerable<TSource> outer, Action<TSource> updator)
        {
            foreach (var item in outer)
            {
                updator(item);
            }


        }
        public static IQueryable<T> SortBy<T>(this IQueryable<T> source, string propertyName)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            // DataSource control passes the sort parameter with a direction 
            // if the direction is descending            
            int descIndex = propertyName.IndexOf(" DESC");

            if (descIndex >= 0)
            {
                propertyName = propertyName.Substring(0, descIndex).Trim();
            }

            if (String.IsNullOrEmpty(propertyName))
            {
                return source;
            }

            ParameterExpression parameter = Expression.Parameter(source.ElementType, String.Empty);
            MemberExpression property = Expression.Property(parameter, propertyName);
            LambdaExpression lambda = Expression.Lambda(property, parameter);

            string methodName = (descIndex < 0) ? "OrderBy" : "OrderByDescending";

            Expression methodCallExpression = Expression.Call(typeof(Queryable), methodName,
                                                new Type[] { source.ElementType, property.Type },
                                                source.Expression, Expression.Quote(lambda));

            return source.Provider.CreateQuery<T>(methodCallExpression);
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

    } 

}