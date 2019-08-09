using AdaptiveBlocks.Speech;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks
{
    [JsonConverter(typeof(BaseAdaptiveBlockConverter))]
    public abstract class BaseAdaptiveBlockElement
    {
        [JsonProperty(Order = int.MinValue)]
        public abstract string Type { get; }

        public abstract BaseAdaptiveBlockElementHints GetHints();

        public abstract IEnumerable<AdaptiveBlockContent> GetFinalBlocks();

        public abstract List<AdaptiveSpeechPrompt> GetSpeechPrompts();

        public abstract IEnumerable<BaseAdaptiveBlockElement> GetChildren();

        public IEnumerable<BaseAdaptiveBlockElement> GetDescendants()
        {
            foreach (var c in GetChildren())
            {
                yield return c;

                foreach (var d in c.GetDescendants())
                {
                    yield return d;
                }
            }
        }
    }

    public abstract class BaseAdaptiveBlockElementHints
    {
        public bool Column { get; set; }

        public bool FactSet { get; set; }
    }


}
