using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks
{
    public class ParseError
    {
        public ParseError(ParseErrorType type, string message)
        {
            Type = type;
            Message = message;
        }

        public ParseErrorType Type { get; set; }
        public string Message { get; set; }
        public int Line { get; set; }
        public ErrorPositionInfo Position { get; set; }
    }

    public enum ParseErrorType
    {
        Error,
        Warning
    }

    public class ErrorPositionInfo
    {
        public int LineNumber { get; set; }
    }
}
