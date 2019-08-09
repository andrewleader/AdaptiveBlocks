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
    public class InteractiveBlockTransformer : BaseElementTransformer<AdaptiveInteractiveBlock>
    {
        public override List<AdaptiveElement> Transform(AdaptiveInteractiveBlock element, AdaptiveBlockTransformContext context)
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

            if (element.Actions.Any())
            {
                AdaptiveColumnSet actionColumnSet = new AdaptiveColumnSet();
                foreach (var action in element.Actions)
                {
                    actionColumnSet.Columns.Add(new AdaptiveColumn()
                    {
                        Items =
                        {
                            new AdaptiveTextBlock
                            {
                                Text = action.Title,
                                Weight = AdaptiveTextWeight.Bolder,
                                Color = AdaptiveTextColor.Accent,
                                HorizontalAlignment = AdaptiveHorizontalAlignment.Center
                            }
                        }
                    });
                }
                answer.Add(actionColumnSet);
            }

            return answer;
        }
    }
}
