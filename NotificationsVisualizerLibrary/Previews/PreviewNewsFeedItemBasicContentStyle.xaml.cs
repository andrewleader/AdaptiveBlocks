using NotificationsVisualizerLibrary.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace NotificationsVisualizerLibrary.Renderers
{
    public sealed partial class PreviewNewsFeedItemBasicContentStyle : UserControl
    {
        public PreviewNewsFeedItemBasicContentStyle()
        {
            this.InitializeComponent();
        }

        internal void Initialize(AdaptiveBinding binding)
        {
            PreviewTileNotification notif = new PreviewTileNotification()
            {
                RequestedTheme = ElementTheme.Light
            };
            notif.InitializeFromXml(TileSize.Large, new PreviewTileVisualElements()
            {
                BackgroundColor = Colors.Transparent
            }, isBrandingVisible: false,
            binding: binding);

            AdaptiveContentContainer.Child = notif;
        }
    }
}
