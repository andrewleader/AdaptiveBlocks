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
    internal sealed partial class PreviewNewsFeedItemContentHeroStyle : UserControl
    {
        public PreviewNewsFeedItemContentHeroStyle()
        {
            this.InitializeComponent();
        }

        internal void Initialize(AdaptiveBinding binding)
        {
            AdaptiveImage heroImage = binding.Container.Children.OfType<AdaptiveImage>().FirstOrDefault(i => i.Placement == Model.Enums.Placement.Hero);
            if (heroImage != null)
            {
                BorderHeroContent.Background = new ImageBrush()
                {
                    ImageSource = ImageHelper.GetBitmap(heroImage.Src),
                    Stretch = Stretch.UniformToFill
                };

                switch (heroImage.HintHeroHeight)
                {
                    case Model.Enums.HintHeroHeight.Double:
                        RowDefinitionHeroContent.Height = new GridLength(RowDefinitionHeroContent.Height.Value * 2);
                        break;
                }
            }

            AdaptiveImage heroLogoImage = binding.Container.Children.OfType<AdaptiveImage>().FirstOrDefault(i => i.Placement == Model.Enums.Placement.HeroLogo);
            if (heroLogoImage != null)
            {
                HeroLogoContainer.Visibility = Visibility.Visible;
                CircleImageHero.Source = ImageHelper.GetBitmap(heroLogoImage.Src);
            }

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

        private void UserControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            GoToHoveredState();
        }

        private void UserControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            GoToDefaultState();
        }

        private void GoToDefaultState()
        {
            VisualStateManager.GoToState(this, "DefaultState", true);
        }

        private void GoToHoveredState()
        {
            VisualStateManager.GoToState(this, "HoveredState", true);
        }
    }
}
