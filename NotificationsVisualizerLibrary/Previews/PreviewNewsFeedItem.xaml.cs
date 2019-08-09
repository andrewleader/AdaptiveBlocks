using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Parsers;
using NotificationsVisualizerLibrary.Renderers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
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

namespace NotificationsVisualizerLibrary
{
    public sealed class PreviewNewsFeedItemProperties
    {
        public Color BackgroundColor { get; set; } = Color.FromArgb(255, 0, 120, 215);

        public Uri Square44x44Logo { get; set; }

        public string DisplayName { get; set; }
    }

    public sealed partial class PreviewNewsFeedItem : UserControl
    {
        private const int HEIGHT_OF_APP_BAR = 54;
        private static XmlTemplateParser _parser = new XmlTemplateParser();

        private static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register("Properties", typeof(PreviewNewsFeedItemProperties), typeof(PreviewNewsFeedItem), new PropertyMetadata(new PreviewNewsFeedItemProperties()));

        public PreviewNewsFeedItemProperties Properties
        {
            get { return GetValue(PropertiesProperty) as PreviewNewsFeedItemProperties; }
            set { SetValue(PropertiesProperty, value); }
        }

        public PreviewNewsFeedItem()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Call this to initialize the item with new content. This can be called multiple times, and will clear the previous content.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public ParseResult Initialize(XmlDocument content)
        {
            // TODO: Should be getting supported features via build/device family, not always getting exploritory.
            // But simply returned exploritory for now just to mock up stuff
            ParseResult result = _parser.ParseNewsFeedItem(content.GetXml(), FeatureSet.GetExperimental());

            if (result.IsOkForRender())
            {
                InitializeContent(result.NewsFeed);
            }

            return result;
        }

        private void InitializeContent(NewsFeedItem newsFeedContent)
        {
            ContentContainer.Child = null;

            if (newsFeedContent == null || newsFeedContent.Visual == null)
                return;

            var binding = newsFeedContent.Visual.Bindings.FirstOrDefault(i => i.Template == Model.Enums.Template.NewsFeedGeneric);
            if (binding == null)
                return;

            if (binding.Container.Children.OfType<AdaptiveImage>().Any(i => i.Placement == Model.Enums.Placement.Hero))
            {
                var heroStyle = new PreviewNewsFeedItemContentHeroStyle();
                heroStyle.Initialize(binding);
                ContentContainer.Child = heroStyle;
            }

            else
            {
                var basicStyle = new PreviewNewsFeedItemBasicContentStyle();
                basicStyle.Initialize(binding);
                ContentContainer.Child = basicStyle;
            }
        }
    }
}
