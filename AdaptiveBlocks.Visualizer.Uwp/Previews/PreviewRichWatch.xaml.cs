using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AdaptiveBlocks.Visualizer.Uwp.Previews
{
    public sealed partial class PreviewRichWatch : UserControl, IPreviewBlockHost
    {
        public PreviewRichWatch()
        {
            this.InitializeComponent();
        }

        public void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            try
            {
                Pages.Children.Clear();
                
                foreach (var b in block.GetFinalBlocks())
                {
                    Pages.Children.Add(new PreviewRichWatchBlock()
                    {
                        DataContext = b
                    });
                }

                if (block.Actions.Any())
                {
                    Pages.Children.Add(new PreviewRichWatchActionsPage()
                    {
                        DataContext = block.GetSimplifiedActions()
                    });
                }
            }
            catch { }
        }
    }
}
