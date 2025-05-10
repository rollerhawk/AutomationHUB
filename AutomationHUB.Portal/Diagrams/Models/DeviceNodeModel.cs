using AutomationHUB.Portal.ViewModels;
using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;

namespace AutomationHUB.Portal.Diagrams.Models
{
    public class DeviceNodeModel: NodeModel
    {
        public DeviceViewModel Model { get; set; }

        public DeviceNodeModel(DeviceViewModel model,Point? position = null) : base(position)
        {
            Model = model;
            AddPort(PortAlignment.Left);
            AddPort(PortAlignment.Right);                 
        }
    }
}
