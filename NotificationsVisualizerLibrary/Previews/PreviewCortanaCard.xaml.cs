using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using NotificationsVisualizerLibrary.Model;
using NotificationsVisualizerLibrary.Parsers;
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

/// <summary>
/// This file is excluded when not in experimental mode
/// </summary>
namespace NotificationsVisualizerLibrary
{
    public sealed partial class PreviewCortanaCard : UserControl
    {
        private static XmlTemplateParser _parser = new XmlTemplateParser();

        public PreviewCortanaCard()
        {
            this.InitializeComponent();
        }

        public bool HasContent { get; private set; }

        /// <summary>
        /// Call this to initialize the Cortana card with new content. This can be called multiple times, and will clear the previous content.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public ParseResult Initialize(XmlDocument content)
        {
            // TODO: Should be getting supported features via build/device family, not always getting exploritory.
            // But simply returned exploritory for now just to mock up stuff
            ParseResult result = _parser.ParseCortana(content.GetXml(), FeatureSet.GetExperimental());

            if (result.IsOkForRender())
            {
                InitializeContent(result.Cortana);
            }

            return result;
        }

        /// <summary>
        /// Call this to initialize the Cortana card with new content. This can be called multiple times, and will clear the previous content.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public ParseResult InitializeFromOneNotification(XmlDocument content)
        {
            ParseResult result = _parser.ParseNotification(content.GetXml());

            if (result.IsOkForRender())
            {
                InitializeContent(result.Notification);
            }

            return result;
        }

        private void InitializeContent(ICortana cortanaContent)
        {
            BorderContentContainer.Child = null;
            HasContent = false;

            if (cortanaContent == null || cortanaContent.Visual == null)
                return;

            var binding = cortanaContent.Visual.Bindings.FirstOrDefault(i => i.Template == Model.Enums.Template.CortanaGeneric);
            if (binding == null)
                return;

            HasContent = true;

            PreviewTileNotification notif = new PreviewTileNotification()
            {
                RequestedTheme = ElementTheme.Light
            };
            notif.InitializeFromXml(TileSize.Large, new PreviewTileVisualElements()
            {
                BackgroundColor = Colors.Transparent
            }, isBrandingVisible: false,
            binding: binding);

            BorderContentContainer.Child = notif;
        }
    }
}
