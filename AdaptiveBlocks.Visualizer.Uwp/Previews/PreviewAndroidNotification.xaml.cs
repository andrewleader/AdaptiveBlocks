using AdaptiveBlocks.Inputs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
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
    public sealed partial class PreviewAndroidNotification : UserControl, IPreviewBlockHost
    {
        public PreviewAndroidNotification()
        {
            this.InitializeComponent();
        }

        public void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            TextBlockTitle.Text = block.Title;
            TextBlockSubtitle.Text = block.Subtitle;

            StackPanelButtons.Children.Clear();
            StackPanelButtons.Visibility = Visibility.Visible;
            QuickReply.Visibility = Visibility.Collapsed;
            foreach (var a in block.GetSimplifiedActions())
            {
                if (a.Inputs.Count == 0 || (a.Inputs.Count == 1 && a.Inputs[0] is AdaptiveTextInputBlock))
                {
                    var b = new Button()
                    {
                        Content = a.Title,
                        Padding = new Thickness(12),
                        Foreground = new SolidColorBrush(Colors.Red),
                        Style = (Style)Resources["TextBlockButtonStyle"],
                        FontWeight = FontWeights.SemiBold
                    };
                    StackPanelButtons.Children.Add(b);
                    b.Click += delegate
                    {
                        if (a.Inputs.Count > 0)
                        {
                            StackPanelButtons.Visibility = Visibility.Collapsed;
                            TextBoxQuickReply.PlaceholderText = a.Title;
                            QuickReply.Visibility = Visibility.Visible;
                        }
                    };
                }
            }
        }

        private void SymbolIcon_Tapped(object sender, TappedRoutedEventArgs e)
        {
            StackPanelButtons.Visibility = Visibility.Visible;
            QuickReply.Visibility = Visibility.Collapsed;
        }
    }
}
