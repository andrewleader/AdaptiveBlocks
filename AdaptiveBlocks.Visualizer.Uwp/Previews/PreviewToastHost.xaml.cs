
using AdaptiveBlocks.Actions;
using AdaptiveBlocks.Attributes;
using AdaptiveBlocks.Inputs;
using AdaptiveBlocks.Transformers.ToastContentTransformer;
using Microsoft.Toolkit.Uwp.Notifications;
using NotificationsVisualizerLibrary;
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

namespace AdaptiveBlocks.Visualizer.Uwp.Previews
{
    public sealed partial class PreviewToastHost : UserControl, IPreviewBlockHost
    {
        public PreviewToastHost()
        {
            this.InitializeComponent();
        }

        public async void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            MyPreviewToast.Properties = new PreviewToastProperties()
            {
                DisplayName = args.AppName,
                Square44x44Logo = args.AppLogo
            };

            ToastContent content = (await new AdaptiveBlockToToastContentTransformer().TransformAsync(sourceBlock)).Result;

            MyPreviewToast.Initialize(content.GetXml());
        }
    }
}
