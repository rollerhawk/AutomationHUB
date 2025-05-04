using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Interfaces;
using Elsa.Expressions.Models;
using Elsa.Extensions;
using Elsa.Workflows;
using Elsa.Workflows.Runtime;
using Elsa.Workflows.Runtime.Activities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Engine.Elsa.Activities
{
    public abstract class AutomationMessageEvent<T> : EventBase<T>, IAutomationMessageEvent
    {
        public string AutomationId { get; set; } = default!;

        protected AutomationMessageEvent() : base() { }

        /// <summary>
        /// Name of the Event that this Activity Listens to
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected override string GetEventName(ExpressionExecutionContext context)
        {
            return AutomationId + "." + typeof(T).Name;
        }

        /// <summary>
        /// Handle Event
        /// </summary>
        /// <param name="context"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        protected abstract override void OnEventReceived(ActivityExecutionContext context, T? input);

        protected override ValueTask ExecuteAsync(ActivityExecutionContext context)
        {
            var eventName = GetEventName(context.ExpressionExecutionContext);
            context.WaitForEvent(eventName, AutomationEventReceivedAsync);
            return default;
        }

        protected virtual async ValueTask AutomationEventReceivedAsync(ActivityExecutionContext context)
        {
            var input = (T?)context.GetEventInput();
            Result.Set(context, input);
            await OnEventReceivedAsync(context, input);
            await context.CompleteActivityAsync();
        }
    }
}
