using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Speech
{
    public class AdaptiveSpeechPrompt
    {
        public List<string> Statements { get; set; } = new List<string>();

        public List<AdaptiveSpeechChoice> Choices { get; set; } = new List<AdaptiveSpeechChoice>();

        public string GetTextToSay()
        {
            List<string> strs = new List<string>();

            strs.AddRange(Statements);

            if (Choices.Any())
            {
                strs.Add(GetSpeechChoicesAsText());
            }

            return string.Join(". ", strs);
        }

        private string GetSpeechChoicesAsText()
        {
            var choices = GetSpeechChoices();

            if (choices.Length == 2)
            {
                return $"You can say \"{choices[0]}\" or \"{choices[1]}\".";
            }

            return "You can say " + string.Join(", ", choices.Take(choices.Length - 1).Select(c => $"\"{c}\"")) + $", or \"{choices.Last()}\".";
        }

        public string[] GetSpeechChoices()
        {
            if (Choices.Count == 0)
            {
                return new string[0];
            }

            return Choices.Select(i => i.Text).Concat(new string[] { "I'm done" }).ToArray();
        }
    }
}
