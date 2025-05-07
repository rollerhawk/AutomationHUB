using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Nats.Configuration
{
    public class NatsOptions
    {
        public string Url { get; set; } = default!;
        public string Prefix { get; set; } = default!;
    }
}
