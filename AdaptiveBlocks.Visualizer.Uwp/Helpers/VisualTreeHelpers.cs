using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace AdaptiveBlocks.Visualizer.Uwp.Helpers
{
    public static class VisualTreeHelpers
    {
        public static T Find<T>(UIElement source) where T : UIElement
        {
            return GetDescendantsAndSelf(source).OfType<T>().FirstOrDefault();
        }

        public static IEnumerable<UIElement> GetDescendantsAndSelf(UIElement item)
        {
            yield return item;
            foreach (var d in GetDescendants(item))
            {
                yield return d;
            }
        }

        public static IEnumerable<UIElement> GetDescendants(UIElement parent)
        {
            foreach (var child in GetChildren(parent))
            {
                yield return child;

                foreach (var descendant in GetDescendants(child))
                {
                    yield return descendant;
                }
            }
        }

        public static IEnumerable<UIElement> GetChildren(UIElement parent)
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < count; i++)
            {
                if (VisualTreeHelper.GetChild(parent, i) is UIElement child)
                {
                    yield return child;
                }
            }
        }
    }
}
