using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AdaptiveBlocks.Speech;

namespace AdaptiveBlocks.Inputs
{
    public class AdaptiveInputsGroup : AdaptiveBlockContent
    {
        public override string Type => "InputsGroup";

        public List<BaseAdaptiveBlockElement> Inputs { get; set; } = new List<BaseAdaptiveBlockElement>();

        public override List<AdaptiveSpeechPrompt> GetSpeechPrompts()
        {
            var answer = base.GetSpeechPrompts();

            foreach (var i in Inputs)
            {
                answer.AddRange(i.GetSpeechPrompts());
            }

            return answer;
        }
    }
}
