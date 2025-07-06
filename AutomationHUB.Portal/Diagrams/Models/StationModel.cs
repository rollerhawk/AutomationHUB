using Blazor.Diagrams.Core.Models;

namespace AutomationHUB.Portal.Diagrams.Models
{
    public class StationModel : GroupModel
    {
        public StationModel(IEnumerable<NodeModel> children, byte padding = 30, bool autoSize = true) : base(children, padding, autoSize)
        {
        }
    }
}
