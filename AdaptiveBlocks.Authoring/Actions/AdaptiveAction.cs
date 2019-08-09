using AdaptiveBlocks.Attributes;
using AdaptiveBlocks.Commands;
using AdaptiveBlocks.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Actions
{
    public class AdaptiveAction : BaseAdaptiveAction
    {
        public string Title { get; set; }

        public override string Type => "Action";

        public AdaptiveBlockAttributionIcon Icon { get; set; }

        public BaseAdaptiveCommand Command { get; set; }

        public override IEnumerable<AdaptiveAction> GetSimplifiedActions()
        {
            yield return this;
        }

        public AdaptiveSpeechChoice CreateSpeechChoice()
        {
            return new AdaptiveSpeechChoice()
            {
                Text = Title,
                FollowUpPrompts = Inputs.SelectMany(i => i.GetSpeechPrompts()).ToList()
            };
        }
    }
}
