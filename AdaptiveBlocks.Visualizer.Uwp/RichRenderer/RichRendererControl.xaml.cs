
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AdaptiveBlocks.Visualizer.Uwp.RichRenderer
{
    public sealed partial class RichRendererControl : UserControl
    {
        public RichRendererControl()
        {
            this.InitializeComponent();
        }



        public AdaptiveBlock AdaptiveBlock
        {
            get { return (AdaptiveBlock)GetValue(AdaptiveBlockProperty); }
            set { SetValue(AdaptiveBlockProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AdaptiveBlock.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AdaptiveBlockProperty =
            DependencyProperty.Register("AdaptiveBlock", typeof(AdaptiveBlock), typeof(RichRendererControl), new PropertyMetadata(null, OnAdaptiveBlockChanged));

        private static void OnAdaptiveBlockChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            (sender as RichRendererControl).OnAdaptiveBlockChanged(args);
        }

        private void OnAdaptiveBlockChanged(DependencyPropertyChangedEventArgs args)
        {
            if (AdaptiveBlock == null)
            {
                BlocksControl.RenderableBlocks = null;
            }
            else
            {
                BlocksControl.RenderableBlocks = CreateRenderableBlocks(AdaptiveBlock).ToArray();
            }
        }

        private List<RenderableBlockData> CreateRenderableBlocks(AdaptiveBlock block)
        {
            List<RenderableBlockData> answer = new List<RenderableBlockData>();

            RenderableBlockData parent = new RenderableBlockData(block);
            answer.Add(parent);

            foreach (var child in block.View.Content.DetailedBlocks)
            {
                bool first = true;
                foreach (var rItem in CreateRenderableBlocks(child))
                {
                    if (first)
                    {
                        parent.Children.Add(rItem);
                        rItem.Parent = parent;
                        first = false;
                    }

                    answer.Add(rItem);
                }
            }

            return answer;
        }
    }
}
