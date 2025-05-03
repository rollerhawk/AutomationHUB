using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Interfaces;
using Elsa.Expressions.Models;
using Elsa.Workflows;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Activities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Engine.Activities.Events
{
    public abstract class AutomationMessageEvent<T>(ILogger logger) : EventBase<T> where T : AutomationMessage
    {
        protected readonly ILogger _logger = logger;

        /// <summary>
        /// Name of the Event that this Activity Listens to
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override string GetEventName(ExpressionExecutionContext context)
        {
            return typeof(T).Name;
        }

        /// <summary>
        /// Handle Event
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected abstract override ValueTask OnEventReceivedAsync(ActivityExecutionContext context, T? input);
    }
}
