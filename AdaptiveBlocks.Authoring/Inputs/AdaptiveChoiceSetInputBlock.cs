using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaptiveBlocks.Speech;

namespace AdaptiveBlocks.Inputs
{
    public class AdaptiveChoiceSetInputBlock : BaseAdaptiveBlockElement
    {
        public override string Type => "ChoiceSet";

        public string Id { get; set; }

        public string Placeholder { get; set; }

        public string Title { get; set; }

        public List<AdaptiveChoiceSetChoice> Choices { get; set; } = new List<AdaptiveChoiceSetChoice>();

        public override IEnumerable<BaseAdaptiveBlockElement> GetChildren()
        {
            return new BaseAdaptiveBlockElement[0];
        }

        public override IEnumerable<AdaptiveBlockContent> GetFinalBlocks()
        {
            return new AdaptiveBlockContent[0];
        }

        public override BaseAdaptiveBlockElementHints GetHints()
        {
            return new AdaptiveBlockHints();
        }

        public override List<AdaptiveSpeechPrompt> GetSpeechPrompts()
        {
            return new List<AdaptiveSpeechPrompt>()
            {
                new AdaptiveSpeechPrompt()
                {
                    Statements =
                    {
                        Placeholder ?? "You can choose from"
                    },
                    Choices = Choices.Select(i => new AdaptiveSpeechChoice()
                    {
                        Text = i.Title
                    }).ToList()
                }
            };
        }
    }

    public class AdaptiveChoiceSetChoice
    {
        public string Title { get; set; }
        public string Value { get; set; }
    }
}
