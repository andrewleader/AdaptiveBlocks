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
    public sealed partial class BlocksControl : UserControl
    {
        public BlocksControl()
        {
            this.InitializeComponent();
        }



        public RenderableBlockData[] RenderableBlocks
        {
            get { return (RenderableBlockData[])GetValue(RenderableBlocksProperty); }
            set { SetValue(RenderableBlocksProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Blocks.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RenderableBlocksProperty =
            DependencyProperty.Register("RenderableBlocks", typeof(RenderableBlockData[]), typeof(BlocksControl), new PropertyMetadata(null));


    }
}
