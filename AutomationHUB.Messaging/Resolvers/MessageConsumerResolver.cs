using AutomationHUB.Messaging.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Resolvers
{
    public class MessageConsumerResolver(IServiceProvider provider) : IMessageConsumerResolver
    {
        private readonly IServiceProvider _provider = provider;

        public async Task ConsumeAsync(AutomationMessage message, CancellationToken ct)
        {
            // ermittle den tatsächlichen CLR-Typ
            var messageType = message.GetType();
            // baue das generic Interface IConsumer<messageType>
            var consumerInterface = typeof(IMessageConsumer<>).MakeGenericType(messageType);
            // löse aus dem Container
            var consumer = _provider.GetService(consumerInterface) ?? throw new 
                InvalidOperationException($"No consumer registered for message type {messageType.Name}");

            // rufe HandleAsync mit Reflection / dynamic auf
            var handleMethod = consumerInterface.GetMethod(nameof(IMessageConsumer<AutomationMessage>.HandleAsync))!;
            await (Task)handleMethod.Invoke(consumer, new object[] { message, ct })!;
        }
    }
}
