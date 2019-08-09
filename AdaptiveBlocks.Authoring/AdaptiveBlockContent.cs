using AdaptiveBlocks.Actions;
using AdaptiveBlocks.Speech;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks
{
    public class AdaptiveBlockContent : BaseAdaptiveBlockElement
    {
        [JsonConverter(typeof(AdaptiveBlockTextConverter))]
        public List<AdaptiveBlockText> Text { get; set; } = new List<AdaptiveBlockText>();

        [JsonIgnore]
        public string Title => Text.FirstOrDefault()?.Text;

        [JsonIgnore]
        public string Subtitle
        {
            get
            {
                if (Text.Count > 1)
                {
                    return string.Join("\n", Text.Skip(1).Select(i => i.Text));
                }

                return null;
            }
        }

        [JsonConverter(typeof(AdaptiveBlockImagesConverter))]
        public List<AdaptiveBlockImage> Images { get; set; } = new List<AdaptiveBlockImage>();

        public List<AdaptiveBlock> DetailedBlocks { get; set; } = new List<AdaptiveBlock>();

        /// <summary>
        /// Author should set either DetailedBlocks or AdditionalBlocks, not both
        /// </summary>
        public IEnumerable<AdaptiveBlock> AdditionalBlocks
        {
            get { return DetailedBlocks.Skip(1).ToArray(); }
            set
            {
                var copy = this.MemberwiseClone() as AdaptiveBlockContent;
                copy.DetailedBlocks = new List<AdaptiveBlock>();

                DetailedBlocks = new List<AdaptiveBlock>();
                DetailedBlocks.Add(new AdaptiveBlock(copy));
                DetailedBlocks.AddRange(value);
            }
        }

        public override string Type => "BlockContent";

        public AdaptiveBlockHints Hints { get; set; } = new AdaptiveBlockHints();

        [JsonIgnore]
        public AdaptiveBlockImage FirstImage => Images.FirstOrDefault();

        public AdaptiveBlockImage GetHeroImage()
        {
            return Images.FirstOrDefault(i => !i.Hints.Category.Contains(AdaptiveBlockImageCategoryHints.Profile)
                && !i.Hints.Category.Contains(AdaptiveBlockImageCategoryHints.Icon));
        }

        [JsonIgnore]
        public AdaptiveBlockImage ProfileImage => GetProfileImage();

        public AdaptiveBlockImage GetProfileImage()
        {
            return Images.FirstOrDefault(i => i.Hints.Category.Contains(AdaptiveBlockImageCategoryHints.Profile))
                ?? Images.FirstOrDefault(i => i.Hints.Category.Contains(AdaptiveBlockImageCategoryHints.Icon));
        }

        [JsonIgnore]
        public AdaptiveBlockImage BackgroundImage => GetBackgroundImage();

        /// <summary>
        /// Excludes thumbnail images, since those often have text and aren't meant for background
        /// </summary>
        /// <returns></returns>
        public AdaptiveBlockImage GetBackgroundImage()
        {
            return Images.FirstOrDefault(i => !i.Hints.Category.Contains(AdaptiveBlockImageCategoryHints.Document)
                && !i.Hints.Category.Contains(AdaptiveBlockImageCategoryHints.Icon));
        }

        public AdaptiveBlockImage GetIconImageOrBestFit()
        {
            return Images.FirstOrDefault(i => i.Hints.Category.Contains(AdaptiveBlockImageCategoryHints.Icon))
                ?? Images.FirstOrDefault(i => i.Hints.Category.Contains(AdaptiveBlockImageCategoryHints.Profile))
                ?? Images.FirstOrDefault();
        }

        public List<BaseAdaptiveAction> Actions { get; set; } = new List<BaseAdaptiveAction>();

        public IEnumerable<List<BaseAdaptiveBlockElement>> GetActionInputGroups()
        {
            return Actions.Where(i => i.Inputs.Any()).Select(i => i.Inputs);
        }

        public IEnumerable<AdaptiveAction> GetSimplifiedActions()
        {
            return Actions.SelectMany(i => i.GetSimplifiedActions());
        }

        public override List<AdaptiveSpeechPrompt> GetSpeechPrompts()
        {
            return GetFinalBlocks().Select(i => i.GetCurrentSpeechPrompt()).ToList();
        }

        private AdaptiveSpeechPrompt GetCurrentSpeechPrompt()
        {
            var prompt = new AdaptiveSpeechPrompt();
            if (Title != null)
            {
                prompt.Statements.Add(Title);
            }
            if (Subtitle != null)
            {
                prompt.Statements.Add(Subtitle);
            }

            var actions = GetSimplifiedActions().ToArray();
            if (actions.Any())
            {
                prompt.Choices = actions.Select(i => i.CreateSpeechChoice()).ToList();
            }

            return prompt;
        }

        public override BaseAdaptiveBlockElementHints GetHints()
        {
            return Hints;
        }

        public override IEnumerable<AdaptiveBlockContent> GetFinalBlocks()
        {
            if (DetailedBlocks.Count > 0)
            {
                return DetailedBlocks.SelectMany(i => i.View?.Content?.GetFinalBlocks());
            }
            else
            {
                return new AdaptiveBlockContent[] { this };
            }
        }

        public override IEnumerable<BaseAdaptiveBlockElement> GetChildren()
        {
            return DetailedBlocks.Select(i => i.View?.Content).Where(i => i != null);
        }

        /// <summary>
        /// Used by RichRendererControl
        /// </summary>
        [JsonIgnore]
        public IEnumerable<AdaptiveBlockContent> FlattenedBlocks
        {
            get { return GetFinalBlocks().ToArray(); }
        }
    }

    public class AdaptiveBlockHints : BaseAdaptiveBlockElementHints
    {
        public List<AdaptiveBlockCategoryHints> Category { get; set; } = new List<AdaptiveBlockCategoryHints>();
    }

    public enum AdaptiveBlockCategoryHints
    {
        Default,
        Featured
    }

    public class AdaptiveBlockTextConverter : CustomCreationConverter<List<AdaptiveBlockText>>
    {
        public override List<AdaptiveBlockText> Create(Type objectType)
        {
            return new List<AdaptiveBlockText>();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                JToken token = JToken.Load(reader);
                return new List<AdaptiveBlockText>() { new AdaptiveBlockText() { Text = token.Value<string>() } };
            }
            else
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
