
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace AdaptiveBlocks.Visualizer.Uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RichRendererPreviewPage : Page
    {
        public static RichRendererPreviewPage Current { get; private set; }

        public AdaptiveBlock CurrentAdaptiveBlock
        {
            get { return (AdaptiveBlock)GetValue(CurrentAdaptiveBlockProperty); }
            set { SetValue(CurrentAdaptiveBlockProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CurrentAdaptiveBlock.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CurrentAdaptiveBlockProperty =
            DependencyProperty.Register("CurrentAdaptiveBlock", typeof(AdaptiveBlock), typeof(RichRendererPreviewPage), new PropertyMetadata(null));

        public RichRendererPreviewPage()
        {
            this.InitializeComponent();

            Current = this;
        }

        public static async void RenderNewBlock(AdaptiveBlock block)
        {
            if (Current != null)
            {
                try
                {
                    await Current.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, delegate
                    {
                        Current.CurrentAdaptiveBlock = block;
                    });
                }
                catch { }
            }
        }
    }
}
