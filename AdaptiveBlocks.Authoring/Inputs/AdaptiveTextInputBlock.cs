using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaptiveBlocks.Speech;

namespace AdaptiveBlocks.Inputs
{
    public class AdaptiveTextInputBlock : BaseAdaptiveBlockElement
    {
        public override string Type => "TextInput";

        public string Id { get; set; }

        public string Placeholder { get; set; }

        public string Title { get; set; }

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
                        Placeholder ?? "Say some text"
                    }
                }
            };
        }
    }
}
