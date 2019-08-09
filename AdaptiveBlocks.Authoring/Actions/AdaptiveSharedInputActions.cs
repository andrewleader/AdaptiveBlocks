using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdaptiveBlocks.Actions
{
    public class AdaptiveSharedInputActions : BaseAdaptiveAction
    {
        public List<AdaptiveAction> Actions { get; set; } = new List<AdaptiveAction>();

        public override string Type => "SharedInputActions";

        public override IEnumerable<AdaptiveAction> GetSimplifiedActions()
        {
            return Actions.Select(i => new AdaptiveAction()
            {
                Title = i.Title,
                Inputs = Inputs
            });
        }
    }
}
