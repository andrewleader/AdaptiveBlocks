using System;
using System.Collections.Generic;
using System.Text;

namespace AdaptiveBlocks.Transformers
{
    public class AdaptiveBlockImageConsumer
    {
        public AdaptiveBlockImage Image { get; private set; }

        public AdaptiveBlockConsumerProperties Properties { get; private set; }

        public AdaptiveBlockImageConsumer(AdaptiveBlockImage image, AdaptiveBlockConsumerProperties properties)
        {
            Image = image;
            Properties = properties;
        }
    }
}
