using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace AdaptiveBlocks.Visualizer.Uwp.RichRenderer
{
    public class DetermineTextBlockFitContainer : Panel
    {
        /// <summary>
        /// Returns true if no content is trimmed at all. Everything fits.
        /// </summary>
        public bool FitsEverything { get; set; }

        public bool FitsComfortably { get; set; }

        protected override Size MeasureOverride(Size availableSize)
        {
            if (Children.Count == 0)
            {
                return new Size(0, 0);
            }

            // If unbounded in either or both directions
            if (double.IsPositiveInfinity(availableSize.Width) || double.IsPositiveInfinity(availableSize.Height))
            {
                foreach (var child in Children)
                {
                    child.Measure(availableSize);
                }

                FitsEverything = Children.All(i => i.DesiredSize.Width <= availableSize.Width && i.DesiredSize.Height <= availableSize.Height);
                FitsComfortably = FitsEverything && Children.All(i => i.DesiredSize.Height * 2 <= Math.Max(i.DesiredSize.Width, availableSize.Width));
                return new Size(Children.Max(i => i.DesiredSize.Width), Children.Max(i => i.DesiredSize.Height));
            }

            // Otherwise, we're bounded in both directions

            // Find out what it would do with infinite height
            foreach (var child in Children)
            {
                child.Measure(new Size(availableSize.Width, double.PositiveInfinity));
            }

            FitsEverything = Children.All(i => i.DesiredSize.Width <= availableSize.Width && i.DesiredSize.Height <= availableSize.Height);

            // If it all fits, then yay!
            if (FitsEverything)
            {
                FitsComfortably = FitsEverything && Children.All(i => i.DesiredSize.Height * 2 <= Math.Max(i.DesiredSize.Width, availableSize.Width));
                return new Size(Children.Max(i => i.DesiredSize.Width), Children.Max(i => i.DesiredSize.Height));
            }

            // Otherwise, re-measure (but leave fits as false)
            foreach (var child in Children)
            {
                child.Measure(availableSize);
            }

            FitsComfortably = FitsEverything && Children.All(i => i.DesiredSize.Height * 2 <= Math.Max(i.DesiredSize.Width, availableSize.Width));
            return new Size(Children.Max(i => i.DesiredSize.Width), Children.Max(i => i.DesiredSize.Height));
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (Children.Count == 0)
            {
                return new Size(0, 0);
            }

            foreach (var child in Children)
            {
                child.Arrange(new Rect(new Point(), finalSize));
            }

            return finalSize;
        }
    }
}
