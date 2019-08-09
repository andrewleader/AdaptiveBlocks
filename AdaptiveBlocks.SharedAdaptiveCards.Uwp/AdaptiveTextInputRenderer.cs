using AdaptiveCards.Rendering.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace AdaptiveBlocks.SharedAdaptiveCards.Uwp
{
    public class AdaptiveTextInputRenderer : IAdaptiveElementRenderer
    {
        private IAdaptiveElementRenderer m_originalRenderer;

        public AdaptiveTextInputRenderer(IAdaptiveElementRenderer originalRenderer)
        {
            m_originalRenderer = originalRenderer;
        }

        public UIElement Render(IAdaptiveCardElement element, AdaptiveRenderContext context, AdaptiveRenderArgs renderArgs)
        {
            UIElement renderedOriginalInput = m_originalRenderer.Render(element, context, renderArgs);
            if (renderedOriginalInput != null)
            {
                try
                {
                    var input = element as AdaptiveTextInput;
                    if (input != null)
                    {
                        JsonObject inlineAction = input.AdditionalProperties.GetNamedObject("inlineAction");
                        if (inlineAction != null)
                        {
                            string icon = inlineAction.GetNamedString("iconUrl");
                            if (icon != null)
                            {
                                var renderedIcon = new Image()
                                {
                                    Source = new BitmapImage(new Uri(icon)),
                                    Width = 30,
                                    Height = 30,
                                    Stretch = Windows.UI.Xaml.Media.Stretch.Uniform,
                                    Margin = new Thickness(12, 0, 0, 0)
                                };
                                Grid.SetColumn(renderedIcon, 1);

                                return new Grid()
                                {
                                    ColumnDefinitions =
                                    {
                                        new ColumnDefinition()
                                        {
                                            Width = new GridLength(1, GridUnitType.Star)
                                        },
                                        new ColumnDefinition()
                                        {
                                            Width = GridLength.Auto
                                        }
                                    },
                                        Children =
                                    {
                                        renderedOriginalInput,
                                        renderedIcon
                                    }
                                };
                            }
                        }
                    }
                }
                catch { }
            }

            return renderedOriginalInput;
        }
    }
}
