using AdaptiveBlocks;
using AdaptiveBlocks.Inputs;
using AdaptiveCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Transformers.AdaptiveCards.ElementTransformers
{
    public class InputsGroupTransformer : BaseElementTransformer<AdaptiveInputsGroup>
    {
        public override List<AdaptiveElement> Transform(AdaptiveInputsGroup element, AdaptiveBlockTransformContext context)
        {
            List<AdaptiveElement> answer = new List<AdaptiveElement>();

            if (!context.Transformer.ElementTransformers.TryGetValue(typeof(AdaptiveBlockContent), out IBaseElemementTransformer blockRenderer))
            {
                return answer;
            }

            answer.AddRange(blockRenderer.Transform(element, context));

            if (element.Inputs.Any())
            {
                foreach (var input in element.Inputs)
                {
                    answer.AddRange(context.Transformer.TransformBlockElementToElements(input, context));
                }
            }

            return answer;
        }
    }
}
