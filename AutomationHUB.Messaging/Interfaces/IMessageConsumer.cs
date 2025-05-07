using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Interfaces
{
    public interface IMessageConsumer<in TMessage>
    where TMessage : AutomationMessage
    {
        Task HandleAsync(TMessage message, CancellationToken ct);
    }
}
