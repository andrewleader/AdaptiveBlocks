using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdaptiveBlocks.Transformers
{
    public static class AdaptiveBlockConsumerExtensions
    {
        public static bool HasMultipleDetailedBlocks(this AdaptiveBlock block)
        {
            var detailedBlocks = block.View?.Content?.DetailedBlocks;
            return detailedBlocks != null && detailedBlocks.Count > 1;
        }

        public static bool HasDetailedBlocks(this AdaptiveBlock block)
        {
            var detailedBlocks = block.View?.Content?.DetailedBlocks;
            return detailedBlocks != null && detailedBlocks.Count > 0;
        }

        public static AdaptiveBlock GetFirstDetailedBlock(this AdaptiveBlock block)
        {
            return block.View.Content.DetailedBlocks.First();
        }

        public static bool HasMoreThanOneBlock(this AdaptiveBlock block)
        {
            var detailedBlocks = block.View?.Content?.DetailedBlocks;
            if (detailedBlocks != null && detailedBlocks.Count > 0)
            {
                if (detailedBlocks.Count > 1)
                {
                    return true;
                }

                return detailedBlocks[0].HasMoreThanOneBlock();
            }

            return false;
        }
    }
}
