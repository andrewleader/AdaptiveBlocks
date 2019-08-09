using AdaptiveBlocks.Visualizer.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AdaptiveBlocks.Visualizer.Uwp.RichRenderer
{
    public class BlocksPanel : Panel
    {
        private const double ColumnSpacing = 8;
        private List<RenderableBlockAndElement> m_allChildren;
        protected override Size MeasureOverride(Size availableSize)
        {
            var children = GetChildren();
            m_allChildren = children;
            // Reset all
            foreach (var child in children)
            {
                child.RenderableBlock.Reset();
            }

            var lowestChildren = children.Where(i => i.RenderableBlock.Children.Count == 0).ToList();

            return MeasureOverride(availableSize, lowestChildren);
        }

        private Size MeasureOverrideOneLevelSimplified(Size availableSize, List<RenderableBlockAndElement> currentChildren)
        {
            // Find the lowest level
            int lowestLevel = currentChildren.Max(i => i.RenderableBlock.GetLevelsDeep());
            if (lowestLevel == 0)
            {
                return new Size(0, 0);
            }

            // Find the last lowest item
            var lowest = currentChildren.Reverse<RenderableBlockAndElement>().First(i => i.RenderableBlock.GetLevelsDeep() == lowestLevel);

            var newSimplifiedParent = m_allChildren.First(i => i.RenderableBlock == lowest.RenderableBlock.Parent);

            int index = currentChildren.FindIndex(i => i.RenderableBlock.Parent == newSimplifiedParent.RenderableBlock);
            while (index < currentChildren.Count && currentChildren[index].RenderableBlock.Parent == newSimplifiedParent.RenderableBlock)
            {
                currentChildren[index].RenderableBlock.IsDropped = true;
                currentChildren.RemoveAt(index);
            }
            currentChildren.Insert(index, newSimplifiedParent);

            return MeasureOverride(availableSize, currentChildren);
        }

        private Size MeasureOverride(Size availableSize, List<RenderableBlockAndElement> children)
        {
            // Reset all
            foreach (var child in children)
            {
                child.RenderableBlock.Reset();
            }

            double consumedHeight = 0;
            double remainingHeight = availableSize.Height;
            double maxWidth = 0;

            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];

                if (child.RenderableBlock.IsColumn)
                {
                    var columnGroup = children.Skip(i).TakeWhile(x => x.RenderableBlock.IsColumn).ToArray();

                    // See how many columns fit
                    double colsRemainingHeight = remainingHeight;
                    double colsConsumedHeight = consumedHeight;
                    double colsMaxWidth = maxWidth;
                    int colsThatFit = MeasureAsCols(columnGroup, availableSize.Width, ref colsRemainingHeight, ref colsConsumedHeight, ref colsMaxWidth);

                    // If they'll all fit, perfect!
                    if (colsThatFit == columnGroup.Length)
                    {
                        remainingHeight = colsRemainingHeight;
                        consumedHeight = colsConsumedHeight;
                        maxWidth = colsMaxWidth;
                        i += columnGroup.Length - 1;
                        foreach (var c in columnGroup)
                        {
                            c.RenderableBlock.IsDropped = false;
                        }
                        continue;
                    }

                    // Otherwise, try render as rows and see if that fits more
                    foreach (var c in columnGroup)
                    {
                        c.RenderableBlock.IsColumn = false;
                    }

                    double rowsRemainingHeight = remainingHeight;
                    double rowsConsumedHeight = consumedHeight;
                    double rowsMaxWidth = maxWidth;
                    int rowsThatFit = MeasureAsRows(columnGroup, availableSize.Width, ref rowsRemainingHeight, ref rowsConsumedHeight, ref rowsMaxWidth);

                    System.Diagnostics.Debug.WriteLine("Rows: " + rowsThatFit + ", Cols: " + colsThatFit);

                    // If any non-optional blocks don't fit, simplify
                    if (columnGroup.Skip(Math.Max(colsThatFit, rowsThatFit)).Any(x => !x.RenderableBlock.BlockSource.Optional))
                    {
                        return MeasureOverrideOneLevelSimplified(availableSize, children);
                    }

                    // If more fit as rows, leave it as rows
                    if (rowsThatFit > colsThatFit)
                    {
                        remainingHeight = rowsRemainingHeight;
                        consumedHeight = rowsConsumedHeight;
                        maxWidth = rowsMaxWidth;
                        i += columnGroup.Length - 1;
                        foreach (var c in columnGroup.Take(rowsThatFit))
                        {
                            c.RenderableBlock.IsDropped = false;
                        }
                        continue;
                    }

                    // Otherwise, revert it back to columns
                    foreach (var c in columnGroup)
                    {
                        c.RenderableBlock.IsColumn = true;
                    }

                    // And re-measure for columns
                    MeasureAsCols(columnGroup.Take(colsThatFit), availableSize.Width, ref remainingHeight, ref consumedHeight, ref maxWidth);
                    i += columnGroup.Length - 1;
                    foreach (var c in columnGroup.Take(colsThatFit))
                    {
                        c.RenderableBlock.IsDropped = false;
                    }
                    continue;
                }

                else if (child.RenderableBlock.IsFactSet)
                {
                    var factSetGroup = children.Skip(i).TakeWhile(x => x.RenderableBlock.IsFactSet).ToArray();

                    double factRemainingHeight = remainingHeight;
                    double factConsumedHeight = consumedHeight;
                    double factMaxWidth = maxWidth;
                    foreach (var f in factSetGroup)
                    {
                        MeasureAsRow(f, availableSize.Width, ref factRemainingHeight, ref factConsumedHeight, ref factMaxWidth);
                    }

                    if (factSetGroup.All(x => VisualTreeHelpers.Find<FactSetItemControl>(x.Element).FitsComfortably()))
                    {
                        foreach (var f in factSetGroup)
                        {
                            f.RenderableBlock.IsDropped = false;
                        }
                        i += factSetGroup.Length - 1;
                        continue;
                    }

                    foreach (var f in factSetGroup)
                    {
                        f.RenderableBlock.IsFactSet = false;
                    }

                    // Let continue to normal rows
                }

                if (MeasureAsRow(child, availableSize.Width, ref remainingHeight, ref consumedHeight, ref maxWidth))
                {
                    child.RenderableBlock.IsDropped = false;
                }
                else
                {
                    // If this block was required
                    if (!child.RenderableBlock.BlockSource.Optional)
                    {
                        return MeasureOverrideOneLevelSimplified(availableSize, children);
                    }

                    break;
                }
            }

            return new Size(maxWidth, consumedHeight);
        }

        private int MeasureAsCols(IEnumerable<RenderableBlockAndElement> children, double availableWidth, ref double remainingHeight, ref double consumedHeight, ref double maxWidth)
        {
            // See if can render all as column, and then render those and then continue loop
            double consumedWidth = 0;
            int countThatFit = 0;
            double maxHeight = 0;
            bool first = true;
            foreach (var c in children)
            {
                // We want to let this render with infinite width, to see what it actually wants to be
                c.Element.Measure(new Size(double.PositiveInfinity, Math.Max(0, remainingHeight)));

                if (consumedWidth + c.Element.DesiredSize.Width + (first ? 0 : ColumnSpacing) <= availableWidth
                    && c.Element.DesiredSize.Height <= remainingHeight)
                {
                    countThatFit++;
                    consumedWidth += c.Element.DesiredSize.Width + (first ? 0 : ColumnSpacing);
                    maxHeight = Math.Max(maxHeight, c.Element.DesiredSize.Height);
                    first = false;
                }
                else
                {
                    break;
                }
            }

            maxWidth = Math.Max(maxWidth, consumedWidth);
            remainingHeight -= maxHeight;
            consumedHeight += maxHeight;

            return countThatFit;
        }

        private int MeasureAsRows(IEnumerable<RenderableBlockAndElement> children, double availableWidth, ref double remainingHeight, ref double consumedHeight, ref double maxWidth)
        {
            int countThatFit = 0;
            foreach (var c in children)
            {
                if (MeasureAsRow(c, availableWidth, ref remainingHeight, ref consumedHeight, ref maxWidth))
                {
                    countThatFit++;
                }
                else
                {
                    break;
                }
            }

            return countThatFit;
        }

        private bool MeasureAsRow(RenderableBlockAndElement child, double availableWidth, ref double remainingHeight, ref double consumedHeight, ref double maxWidth)
        {
            if (remainingHeight <= 0)
            {
                return false;
            }

            child.Element.Measure(new Size(availableWidth, double.PositiveInfinity));
            remainingHeight -= child.Element.DesiredSize.Height;
            if (remainingHeight >= 0)
            {
                consumedHeight += child.Element.DesiredSize.Height;
                maxWidth = Math.Max(maxWidth, child.Element.DesiredSize.Width);
                return true;
            }

            return false;
        }

        private class RenderableBlockAndElement
        {
            public RenderableBlockData RenderableBlock { get; set; }
            public UIElement Element { get; set; }
        }

        private List<RenderableBlockAndElement> GetChildren()
        {
            return Children.OfType<FrameworkElement>().Where(i => i.DataContext is RenderableBlockData).Select(i => new RenderableBlockAndElement()
            {
                RenderableBlock = i.DataContext as RenderableBlockData,
                Element = i
            }).ToList();
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var children = GetChildren();

            double y = 0;
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];

                if (child.RenderableBlock.IsDropped)
                {
                    child.Element.Arrange(new Rect(x: 0, y: y, width: 0, height: 0));
                    continue;
                }

                if (child.RenderableBlock.IsColumn)
                {
                    var columnGroup = children.Skip(i).TakeWhile(c => c.RenderableBlock.IsColumn).ToArray();
                    var columnGroupUndropped = columnGroup.Where(c => !c.RenderableBlock.IsDropped).ToArray();

                    // See what the width would be if equally divided
                    double equalWidth = (finalSize.Width - ColumnSpacing * (columnGroupUndropped.Length - 1)) / columnGroupUndropped.Length;

                    // If they all can fit into equal width columns
                    bool useEqualWidth = false;
                    if (columnGroupUndropped.All(c => c.Element.DesiredSize.Width <= equalWidth))
                    {
                        // Make that so
                        useEqualWidth = true;
                    }

                    // Render as columns
                    double x = 0;
                    foreach (var c in columnGroup)
                    {
                        if (c.RenderableBlock.IsDropped)
                        {
                            c.Element.Arrange(new Rect(x: x, y: y, width: 0, height: 0));
                            continue;
                        }

                        c.Element.Arrange(new Rect(
                            x: x,
                            y: y,
                            width: useEqualWidth ? equalWidth : c.Element.DesiredSize.Width,
                            height: c.Element.DesiredSize.Height));

                        x += (useEqualWidth ? equalWidth : c.Element.DesiredSize.Width) + ColumnSpacing;
                    }

                    y += columnGroupUndropped.Max(c => c.Element.DesiredSize.Height);

                    i += columnGroup.Length - 1;
                }

                // Otherwise, render as rows
                else
                {
                    child.Element.Arrange(new Rect(
                        x: 0,
                        y: y,
                        width: finalSize.Width,
                        height: child.Element.DesiredSize.Height));

                    y += child.Element.DesiredSize.Height;
                }

                // Padding between rows
                y += 8;
            }

            return finalSize;
        }
    }
}
