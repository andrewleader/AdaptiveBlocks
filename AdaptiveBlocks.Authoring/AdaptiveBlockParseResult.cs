using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks
{
    public class AdaptiveBlockParseResult
    {
        public IList<ParseError> Errors { get; private set; } = new List<ParseError>();

        public AdaptiveBlock Block { get; set; }
    }
}
