using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Attributes
{
    public class AdaptiveThemes
    {
        public const string Light = "light";
        public const string Dark = "dark";
        public const string AllBlack = "allBlack";
        public const string AllWhite = "allWhite";
        public const string Tinting = "tinting";
        public const string Accent = "accent";
    }

    [JsonConverter(typeof(JsonStringToObjectConverter<AdaptiveBlockAttributionIcon>))]
    public class AdaptiveBlockAttributionIcon
    {
        [JsonImplicitText]
        public string Url { get; set; }

        public string[] SupportedThemes { get; set; }

        public AdaptiveBlockAttributionIcon AltIcon { get; set; }

        public AdaptiveBlockAttributionIcon GetIconForTheme(string theme)
        {
            if (SupportedThemes != null && SupportedThemes.Contains(theme))
            {
                return this;
            }

            // If not specified, it's implicitly "light"
            if (theme == AdaptiveThemes.Light && (SupportedThemes == null || SupportedThemes.Length == 0))
            {
                return this;
            }

            return AltIcon?.GetIconForTheme(theme);
        }
    }
}
