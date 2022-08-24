using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BruteForceGearsetGenerator.Utilities
{
    public static class UsefulFunctions
    {

        public static IEnumerable<IEnumerable<T>> GetPermutationsWithRept<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutationsWithRept(list, length - 1)
                .SelectMany(t => list,
                    (t1, t2) => t1.Concat(new T[] { t2 }));
        }

        public static IEnumerable<IEnumerable<T>> GetPermutationsWithoutRept<T>(IList<T> items, int length)
        {
            if (length == 0 || !items.Any()) return new List<List<T>> { new List<T>() };
            return from item in items.Distinct()
                   from permutation in GetPermutationsWithoutRept(items.Where(i => !EqualityComparer<T>.Default.Equals(i, item)).ToList(), length - 1)
                   select Prepend(item, permutation);
        }

        public static IEnumerable<T> Prepend<T>(T first, IEnumerable<T> rest)
        {
            yield return first;
            foreach (var item in rest) yield return item;
        }


        public static string RemoveWhitespace(string s)
        {
            return String.Concat(s.Where(c => !Char.IsWhiteSpace(c)));
        }
    }
}
