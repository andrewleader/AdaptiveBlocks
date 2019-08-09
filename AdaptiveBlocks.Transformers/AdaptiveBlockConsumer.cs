using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdaptiveBlocks.Transformers
{
    public class AdaptiveBlockConsumer
    {
        public AdaptiveBlock Block { get; private set; }

        public AdaptiveBlockConsumerProperties Properties { get; private set; }

        private AdaptiveBlockConsumer m_parentConsumer;
        private AdaptiveBlockContent m_content;

        /// <summary>
        /// Note that the block WILL be modified.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="properties"></param>
        public AdaptiveBlockConsumer(AdaptiveBlock block, AdaptiveBlockConsumerProperties properties)
        {
            Block = block;
            m_content = block.View?.Content;
        }

        public bool ConsumeContent(out AdaptiveBlockContentConsumer contentConsumer)
        {
            if (Block.View?.Content != null)
            {
                contentConsumer = new AdaptiveBlockContentConsumer(Block.View.Content, Properties);
                Block.View.Content = null;
                return true;
            }

            contentConsumer = null;
            return false;
        }



        public AdaptiveBlockConsumer ConsumeNextBlock()
        {
            if (Properties.AllowsDetailedBlocks)
            {
                var detailed = Block?.View?.Content?.DetailedBlocks;
                if (detailed != null && detailed.Count > 0)
                {
                    var answer = detailed[0];
                    detailed.RemoveAt(0);
                    if (detailed.Count == 0)
                    {
                        Block = null;
                    }
                    return new AdaptiveBlockConsumer(answer, Properties)
                    {
                        m_parentConsumer = this
                    }.ConsumeNextBlock();
                }
            }

            if (Block != null)
            {
                return this;
            }

            return null;
        }
    }
}
