using AutomationHUB.Messaging.Devices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Interfaces
{
    public interface IPublisher
    {
        string Publish(INATSPublishable message);
    }
}
