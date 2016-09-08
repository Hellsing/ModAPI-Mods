using System;
using System.Collections.Generic;

namespace GriefClientPro.Utils
{
    public static class Extensions
    {
        private static readonly Random Random = new Random(DateTime.Now.Millisecond);

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Random.Next(n + 1);
                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
