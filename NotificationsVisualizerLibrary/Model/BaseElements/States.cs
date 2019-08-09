using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NotificationsVisualizerLibrary.Parsers;

namespace NotificationsVisualizerLibrary.Model.BaseElements
{
    internal class States : AdaptiveParentElement
    {
        public const string ELEMENT_NAME = "states";

        public States(NotificationType context, FeatureSet supportedFeatures) : base(context, supportedFeatures) { }

        internal IList<State> Children { get; private set; } = new List<State>();

        internal void Parse(ParseResult result, XElement node)
        {
            AttributesHelper attributes = new AttributesHelper(node.Attributes());
            
            ParseKnownAttributes(node, attributes, result);

            HandleRemainingAttributes(attributes, result);

            // 0-n children
            foreach (XElement child in node.Elements())
            {
                if (!result.IsOkForRender())
                    break;

                try
                {
                    HandleChild(result, child);
                }

                catch (IncompleteElementException)
                {

                }
            }
        }

        internal void ParseKnownAttributes(XElement node, AttributesHelper attributes, ParseResult result)
        {
            // Nothing right now
        }

        protected virtual void HandleChild(ParseResult result, XElement child)
        {
            switch (child.Name.LocalName.ToLower())
            {
                case State.ELEMENT_NAME:

                    var state = new State(Context, SupportedFeatures);
                    state.Parse(result, child);

                    if (!result.IsOkForRender())
                        return;

                    Children.Add(state);
                    state.Parent = this;

                    break;


                default:
                    result.AddWarning($"Invalid child {child.Name.LocalName} under {ELEMENT_NAME} element.", GetErrorPositionInfo(child));
                    break;
            }
        }

        protected override IEnumerable<string> GetAttributesNotSupportedByVisualizer()
        {
            return new string[] { };
        }

        internal override IEnumerable<AdaptiveChildElement> GetAllChildren()
        {
            foreach (var i in Children)
                yield return i;
        }
    }
}
