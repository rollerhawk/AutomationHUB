using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Interfaces
{
    public interface IPublisher
    {
        void Publish(string subject, byte[] message);
    }

    public interface IPublisher<T> : IPublisher
    {
        void Publish(string subject, T message);
    }
}
