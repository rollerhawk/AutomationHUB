using AutomationHUB.Messaging.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Registry
{
    [NATSDomain(Domain = "registry")]
    public abstract class RegistryMessage() : AutomationMessage
    {
        public abstract override required string Entity { get; set; }
        public abstract override required string Id { get; set; }
    }
}
