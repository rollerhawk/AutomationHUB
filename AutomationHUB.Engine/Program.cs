using AutomationHUB.Engine;
using AutomationHUB.Engine.Activities.Events;
using AutomationHUB.Engine.Adapters;
using AutomationHUB.Engine.MessageConsumers;
using AutomationHUB.Engine.Tests;
using AutomationHUB.Messaging.Interfaces;
using AutomationHUB.Messaging.Nats;
using AutomationHUB.Messaging.Nats.Extensions;
using AutomationHUB.Messaging.Resolvers;
using Elsa.Extensions;
using Elsa.Workflows.Runtime;


var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddElsa(elsa =>
    {
        elsa.AddWorkflow<TestScannerWorkflow>()
        .AddActivity<DeviceMessageEvent>();
    });

builder.Services.AddHostedService<SubscriberTriggerHostedService>()
                .AddNats(builder.Configuration)
                .AddSingleton<ISubscriber, NatsSubscriber>();

builder.Services.Scan(scan => scan
  .FromAssemblyOf<DeviceMessageElsaConsumer>()
  .AddClasses(classes => classes.AssignableTo(typeof(IMessageConsumer<>)))
  .AsImplementedInterfaces()
  .WithScopedLifetime());


var host = builder.Build();

host.Run();
