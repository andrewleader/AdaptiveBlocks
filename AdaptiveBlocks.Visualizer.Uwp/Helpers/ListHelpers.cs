using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Visualizer.Uwp.Helpers
{
    public static class ListHelpers
    {
        public static void MakeListLike<T>(this IList<T> list, IEnumerable<T> makeLike)
        {
            // Simple implementation for now
            list.Clear();
            foreach (var val in makeLike)
            {
                list.Add(val);
            }
        }
    }
}
