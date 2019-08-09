using AdaptiveBlocks.Actions;
using AdaptiveBlocks.SharedAdaptiveCards.Uwp;
using AdaptiveBlocks.Transformers.AdaptiveCards;
using AdaptiveBlocks.Visualizer.Uwp.Renderer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class PreviewRichWatchActionsPage : UserControl
    {
        public PreviewRichWatchActionsPage()
        {
            this.InitializeComponent();

            DataContextChanged += PreviewRichWatchActionsPage_DataContextChanged;
        }

        private void PreviewRichWatchActionsPage_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            InputCollection.Visibility = Visibility.Collapsed;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            AdaptiveAction action = (sender as FrameworkElement).DataContext as AdaptiveAction;

            if (action != null && action.Inputs.Any())
            {
                var transformer = new AdaptiveBlockToCardTransformer();
                var sharedCard = (await transformer.TransformAsync(new AdaptiveBlock(new AdaptiveBlockContent()
                {
                    Actions =
                    {
                        action
                    }
                }))).Result;

                InputCollectionFields.Child = SharedAdaptiveCardRenderer.RenderFromCard(sharedCard);
                InputCollection.Visibility = Visibility.Visible;
            }
            else
            {
                await new MessageDialog("Action performed!").ShowAsync();
            }
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            InputCollection.Visibility = Visibility.Collapsed;
        }
    }
}
