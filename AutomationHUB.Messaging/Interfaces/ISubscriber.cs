﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Messaging.Interfaces
{
    public interface ISubscriber
    {
        /// <summary>
        /// Registriert einen Handler für alle Nachrichten auf dem gegebenen Subject-Pattern.
        /// </summary>
        void Subscribe(string subject, Func<byte[], CancellationToken, Task> handler);
    }

    public interface ISubscriber<T> : ISubscriber where T : INATSPublishable
    {
        string GetSubject(T message);

        void Subscribe(Func<T, CancellationToken, Task> handler);
    }

}
