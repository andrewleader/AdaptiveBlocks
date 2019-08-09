using AdaptiveBlocks.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace AdaptiveBlocks.RichRenderer
{
    public class RichBlockControl : Panel
    {
        private BlockControl m_summaryBlock = new BlockControl();
        private BlocksControl m_detailedBlocks = new BlocksControl();

        public RichBlockControl()
        {
            Children.Add(m_summaryBlock);
            Children.Add(m_detailedBlocks);
        }

        public RenderableBlockData Block
        {
            get { return (RenderableBlockData)GetValue(BlockProperty); }
            set { SetValue(BlockProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Block.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BlockProperty =
            DependencyProperty.Register("Block", typeof(RenderableBlockData), typeof(RichBlockControl), new PropertyMetadata(null, OnBlockChanged));

        private static void OnBlockChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as RichBlockControl).OnBlockChanged(args);
        }

        private void OnBlockChanged(DependencyPropertyChangedEventArgs args)
        {
            if (Block == null)
            {
                m_summaryBlock.DataContext = null;
                m_detailedBlocks.RenderableBlocks = null;
                return;
            }

            m_summaryBlock.DataContext = Block;

            if (Block.Block.DetailedBlocks != null)
            {
                m_detailedBlocks.RenderableBlocks = Block.Block.DetailedBlocks.OfType<AdaptiveBlock>().Select(i => new RenderableBlockData(i)).ToArray();
            }
            else
            {
                m_detailedBlocks.RenderableBlocks = null;
            }
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            m_detailedBlocks.Measure(availableSize);
            return m_detailedBlocks.DesiredSize;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            m_detailedBlocks.Arrange(new Rect(new Point(), finalSize));
            return finalSize;
        }
    }
}
