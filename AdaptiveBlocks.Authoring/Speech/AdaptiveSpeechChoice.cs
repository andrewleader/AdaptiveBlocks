using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Speech
{
    public class AdaptiveSpeechChoice
    {
        public string Text { get; set; }

        public List<AdaptiveSpeechPrompt> FollowUpPrompts { get; set; } = new List<AdaptiveSpeechPrompt>();
    }
}
