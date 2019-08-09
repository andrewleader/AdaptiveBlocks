using AdaptiveBlocks;
using AdaptiveCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Transformers.AdaptiveCards.ElementTransformers
{
    public abstract class BaseElementTransformer<T> : IBaseElemementTransformer
        where T : BaseAdaptiveBlockElement
    {
        public abstract List<AdaptiveElement> Transform(T element, AdaptiveBlockTransformContext context);

        public List<AdaptiveElement> Transform(BaseAdaptiveBlockElement element, AdaptiveBlockTransformContext context)
        {
            return Transform(element as T, context);
        }
    }

    public interface IBaseElemementTransformer
    {
        List<AdaptiveElement> Transform(BaseAdaptiveBlockElement element, AdaptiveBlockTransformContext context);
    }
}
