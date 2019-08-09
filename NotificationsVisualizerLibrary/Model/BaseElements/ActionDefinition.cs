using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    internal class ActionDefinition : AdaptiveChildElement
    {
        public ActionDefinition(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        internal const string ATTR_ID = "id";
        internal const string ATTR_ARGUMENTS = "arguments";
        internal const string ATTR_ACTIVATIONTYPE = "activationType";

        public string Id { get; set; }

        public string Arguments { get; set; }

        public ActivationType ActivationType { get; set; } = ActivationType.Foreground;

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { };
        }

        internal void Parse(ParseResult result, XElement node)
        {
            AttributesHelper attributes = new AttributesHelper(node.Attributes());

            ParseKnownAttributes(node, attributes, result);

            HandleRemainingAttributes(attributes, result);
        }

        internal virtual void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // id is required
            XAttribute attrId = attributes.PopAttribute(ATTR_ID);
            if (attrId == null)
            {
                result.AddError("id attribute on action element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            this.Id = attrId.Value;


            // arguments is required
            XAttribute attrArguments = attributes.PopAttribute(ATTR_ARGUMENTS);
            if (attrArguments == null)
            {
                result.AddErrorButRenderAllowed("arguments attribute on action element is required.", XmlTemplateParser.GetErrorPositionInfo(node));
                throw new IncompleteElementException();
            }

            this.Arguments = attrArguments.Value;


            // activationType is optional
            ActivationType type;
            if (TryParseEnum(result, attributes, ATTR_ACTIVATIONTYPE, out type))
                this.ActivationType = type;
        }
    }
}
