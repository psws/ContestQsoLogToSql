using System;
using System.Collections.Generic;
using System.Linq;
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
    } 

}