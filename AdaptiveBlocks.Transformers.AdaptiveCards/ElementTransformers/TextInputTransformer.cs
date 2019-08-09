using AdaptiveBlocks.Inputs;
using AdaptiveCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Transformers.AdaptiveCards.ElementTransformers
{
    public class TextInputTransformer : BaseElementTransformer<AdaptiveTextInputBlock>
    {
        public override List<AdaptiveElement> Transform(AdaptiveTextInputBlock element, AdaptiveBlockTransformContext context)
        {
            return Transform(element);
        }

        public static List<AdaptiveElement> Transform(AdaptiveTextInputBlock element)
        {
            List<AdaptiveElement> answer = new List<AdaptiveElement>();

            if (element.Title != null)
            {
                answer.Add(new AdaptiveTextBlock()
                {
                    Text = element.Title,
                    Wrap = true
                });
            }

            AdaptiveTextInput input = new AdaptiveTextInput()
            {
                Id = element.Id,
                Placeholder = element.Placeholder
            };

            answer.Add(input);

            return answer;
        }
    }
}
