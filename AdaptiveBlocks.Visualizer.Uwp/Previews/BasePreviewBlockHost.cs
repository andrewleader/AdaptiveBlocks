using AdaptiveBlocks.SharedAdaptiveCards.Uwp;
using AdaptiveBlocks.Transformers.AdaptiveCards;

using AdaptiveBlocks.Visualizer.Uwp.Renderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace AdaptiveBlocks.Visualizer.Uwp.Previews
{
    public abstract class BasePreviewBlockHost : Grid, IPreviewBlockHost
    {
        private AdaptiveBlockToCardTransformer m_renderer;

        public BasePreviewBlockHost()
        {
            m_renderer = new AdaptiveBlockToCardTransformer()
            {
                Properties = GetProperties()
            };
        }

        protected abstract AdaptiveBlockToCardTransformerProperties GetProperties();

        public async void Update(AdaptiveBlockContent block, AdaptiveBlock sourceBlock, PreviewBlockHostViewModel args)
        {
            this.Children.Clear();

            try
            {
                AdaptiveCards.AdaptiveCard card = (await m_renderer.TransformAsync(sourceBlock)).Result;

                this.Children.Add(SharedAdaptiveCardRenderer.RenderFromCard(card));
            }
            catch { }
        }
    }
}
