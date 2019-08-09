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

namespace AdaptiveBlocks.Visualizer.Uwp.Views.PreviewPanes
{
    public sealed partial class BlocksPreviewPane : UserControl
    {
        public BlocksPreviewPane()
        {
            this.InitializeComponent();

            _prevVerticalOffset = PrevVerticalOffset;
            _prevContentHeight = PrevContentHeight;
        }

        public static double PrevVerticalOffset;
        public static double PrevContentHeight;
        private double _prevVerticalOffset;
        private double _prevContentHeight;

        private void MyStackPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double newContentHeight = MyScrollViewer.ScrollableHeight + MyScrollViewer.ViewportHeight;

            if (_prevContentHeight > 0)
            {
                double ratio = newContentHeight / _prevContentHeight;

                double newVerticalOffset = _prevVerticalOffset * ratio;
                MyScrollViewer.ChangeView(null, newVerticalOffset, null, true);
            }
        }

        private void MyScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (MyScrollViewer.ScrollableHeight + MyScrollViewer.ViewportHeight > 0)
            {
                PrevVerticalOffset = MyScrollViewer.VerticalOffset;
                PrevContentHeight = MyScrollViewer.ScrollableHeight + MyScrollViewer.ViewportHeight;
                _prevContentHeight = PrevContentHeight;
                _prevVerticalOffset = PrevVerticalOffset;
            }
        }
    }
}
