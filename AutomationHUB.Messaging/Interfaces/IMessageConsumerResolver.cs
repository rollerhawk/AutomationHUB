using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Interfaces
{
    public interface IMessageConsumerResolver
    {
        Task ConsumeAsync(AutomationMessage message, CancellationToken ct);
    }
}
