using AdaptiveBlocks.Transformers.AdaptiveCards;
using AdaptiveCards;
using Newtonsoft.Json;
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
    public sealed partial class PreviewToastCardHost : UserControl, IPreviewBlockHost
    {
        public PreviewToastCardHost()
        {
            this.InitializeComponent();
        }

        private bool m_isExpanded = true;
        public bool IsExpanded
        {
            get { return m_isExpanded; }
            set
            {
                if (m_isExpanded != value)
                {
                    m_isExpanded = value;

                    if (m_currSourceBlock != null)
                    {
                        try
                        {
                            Update(m_currSourceBlock.View?.Content, m_currSourceBlock, m_currHostViewModel);
                        }
                        catch { }
                    }
                }
            }
        }

        private AdaptiveBlock m_currSourceBlock;
        private PreviewBlockHostViewModel m_currHostViewModel;
        public async void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            try
            {
                m_currSourceBlock = sourceBlock;
                m_currHostViewModel = args;

                MyPreviewToast.Properties = new PreviewToastProperties()
                {
                    DisplayName = args.AppName,
                    Square44x44Logo = args.AppLogo
                };



                AdaptiveBlockContent firstFinalBlock;

                if (IsExpanded)
                {
                    firstFinalBlock = sourceBlock.GetFirstFinalBlock().View.Content;
                }
                else
                {
                    firstFinalBlock = block;
                }

                string title = firstFinalBlock.Title;
                string subtitle = firstFinalBlock.Subtitle;

                var originalText = firstFinalBlock.Text.ToArray();
                var originalImages = firstFinalBlock.Images.ToArray();

                firstFinalBlock.Text.Clear();

                var firstHeroImg = firstFinalBlock.GetHeroImage();

                var firstProfileImg = firstFinalBlock.GetProfileImage();

                firstFinalBlock.Images.Clear();

                AdaptiveCard card;
                if (IsExpanded)
                {
                    var transformer = new AdaptiveBlockToCardTransformer();
                    card = (await transformer.TransformAsync(sourceBlock)).Result;
                }
                else
                {
                    card = new AdaptiveCard("1.0");
                }

                firstFinalBlock.Text.AddRange(originalText);
                firstFinalBlock.Images.AddRange(originalImages);

                card.AdditionalProperties["title"] = new Dictionary<string, string>()
            {
                { "text", title }
            };
                if (subtitle != null)
                {
                    card.AdditionalProperties["subtitle"] = new Dictionary<string, string>()
                {
                    { "text", subtitle }
                };
                }

                Dictionary<string, Dictionary<string, string>> imagesObject = new Dictionary<string, Dictionary<string, string>>();

                if (firstProfileImg != null)
                {
                    imagesObject["profile"] = new Dictionary<string, string>()
                {
                    { "url", firstProfileImg.Url },
                    { "style", "person" }
                };
                }

                if (firstHeroImg != null)
                {
                    imagesObject["card"] = new Dictionary<string, string>()
                {
                    { "url", firstHeroImg.Url }
                };
                }

                if (imagesObject.Count > 0)
                {
                    card.AdditionalProperties["images"] = imagesObject;
                }

                MyPreviewToast.InitializeFromJson(card.ToJson(), new NotificationsVisualizerLibrary.PreviewNotificationData());
            }
            catch { }
        }
    }
}
