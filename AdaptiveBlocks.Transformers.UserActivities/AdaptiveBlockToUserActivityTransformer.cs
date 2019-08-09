using AdaptiveBlocks.Attributes;
using AdaptiveCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserActivities;
using Windows.UI.Shell;

namespace AdaptiveBlocks.Transformers.UserActivities
{
    public class AdaptiveBlockToUserActivityTransformer : AdaptiveBlockTransformer<UserActivity>
    {
        private UserActivity m_activity;
        public AdaptiveBlockToUserActivityTransformer(UserActivity activity)
        {
            m_activity = activity;
        }

        protected override Task TransformAsync(AdaptiveBlock block, AdaptiveBlockTransformResult<UserActivity> result)
        {
            var content = block.View?.Content;
            if (content != null)
            {
                if (content.Title != null)
                {
                    m_activity.VisualElements.DisplayText = content.Title;
                }

                if (content.Subtitle != null)
                {
                    m_activity.VisualElements.Description = content.Subtitle;
                }
            }

            var cardContent = block.View?.RichContent?.FirstOrDefault(i => i.ContentType == "application/vnd.microsoft.card.adaptive" && i.TargetedExperiences != null && i.TargetedExperiences.Contains("Microsoft.UserActivities.Hero"))?.Content;
            if (cardContent != null)
            {
                m_activity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(cardContent.ToString());
            }
            else if (content != null)
            {
                var backgroundImageRequest = AdaptiveBlockContentConsumer.ImageMatchRequest.ForBackgroundImage();

                AdaptiveBlockContentConsumer.MatchImages(content, backgroundImageRequest);

                if (backgroundImageRequest.ImageResult != null)
                {
                    var adaptiveCard = new AdaptiveCard("1.0");
                    adaptiveCard.BackgroundImage = new Uri(backgroundImageRequest.ImageResult.Url);

                    if (content.Title != null)
                    {
                        adaptiveCard.Body.Add(new AdaptiveTextBlock()
                        {
                            Text = content.Title,
                            Size = AdaptiveTextSize.Large,
                            Weight = AdaptiveTextWeight.Bolder,
                            Wrap = true,
                            MaxLines = 3
                        });
                    }

                    if (content.Subtitle != null)
                    {
                        adaptiveCard.Body.Add(new AdaptiveTextBlock()
                        {
                            Text = content.Subtitle,
                            Wrap = true,
                            MaxLines = 3
                        });
                    }

                    m_activity.VisualElements.Content = AdaptiveCardBuilder.CreateAdaptiveCardFromJson(adaptiveCard.ToJson());
                }
            }

            if (block.View?.Attributes != null)
            {
                var attributes = block.View.Attributes;

                if (attributes.AttributionText != null)
                {
                    m_activity.VisualElements.AttributionDisplayText = attributes.AttributionText.Text;
                }

                var attrIcon = attributes.AttributionIcon?.GetIconForTheme(AdaptiveThemes.Dark);
                if (attrIcon != null)
                {
                    m_activity.VisualElements.Attribution = new UserActivityAttribution(new Uri(attrIcon.Url));
                }
            }

            result.Result = m_activity;
            return Task.CompletedTask;
        }
    }
}
