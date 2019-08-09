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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace AdaptiveBlocks.Visualizer.Uwp.Previews
{
    public sealed partial class PreviewGitHubPullRequest : UserControl, IPreviewBlockHost
    {
        public PreviewGitHubPullRequest()
        {
            this.InitializeComponent();
        }

        public void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            DataContext = block;

            var icon = block.GetIconImageOrBestFit();
            if (icon?.Url != null)
            {
                IconImageBrush.ImageSource = new BitmapImage(new Uri(icon.Url));
                IconImage.Visibility = Visibility.Visible;

                IconImage.Margin = new Thickness(icon.Hints.Category.Contains(AdaptiveBlockImageCategoryHints.Icon) ? 8 : 0);

                if (icon.BackgroundColor != null)
                {
                    //IconBackground.Fill = new SolidColorBrush(icon.BackgroundColor);
                }
                else
                {
                    IconBackground.Fill = new SolidColorBrush(Colors.LightGray);
                }
            }
            else
            {
                IconImage.Visibility = Visibility.Collapsed;
            }

            var actions = block.GetSimplifiedActions();
            if (actions.Any())
            {
                Buttons.Visibility = Visibility.Visible;

                FirstButton.Text = actions.First().Title;

                if (actions.Count() > 1)
                {
                    MoreButton.Visibility = Visibility.Visible;
                }
                else
                {
                    MoreButton.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                Buttons.Visibility = Visibility.Collapsed;
            }
        }
    }
}
