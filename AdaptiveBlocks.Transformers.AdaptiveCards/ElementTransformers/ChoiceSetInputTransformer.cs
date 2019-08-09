using AdaptiveBlocks.Inputs;
using AdaptiveCards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Transformers.AdaptiveCards.ElementTransformers
{
    public class ChoiceSetInputTransformer : BaseElementTransformer<AdaptiveChoiceSetInputBlock>
    {
        public override List<AdaptiveElement> Transform(AdaptiveChoiceSetInputBlock element, AdaptiveBlockTransformContext context)
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

            AdaptiveChoiceSetInput choiceSet = new AdaptiveChoiceSetInput()
            {
                Id = element.Id,
                Style = AdaptiveChoiceInputStyle.Compact
            };
            foreach (var c in element.Choices)
            {
                choiceSet.Choices.Add(new AdaptiveChoice()
                {
                    Title = c.Title,
                    Value = c.Value
                });
            }

            answer.Add(choiceSet);
            return answer;
        }
    }
}
