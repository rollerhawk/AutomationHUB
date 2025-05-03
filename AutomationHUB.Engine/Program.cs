using AutomationHUB.Engine;
using AutomationHUB.Engine.MessageConsumers;
using AutomationHUB.Messaging.Interfaces;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .AddHostedService<Worker>();

builder.Services.Scan(scan => scan
  .FromAssemblyOf<DeviceMessageElsaConsumer>()
  .AddClasses(classes => classes.AssignableTo(typeof(IMessageConsumer<>)))
  .AsImplementedInterfaces()
  .WithScopedLifetime());

var host = builder.Build();
host.Run();
