using AutomationHUB.Engine.Api.Contracts;
using AutomationHUB.Engine.Api.Services;
using AutomationHUB.Engine.Api.SignalR;
using AutomationHUB.Engine.Elsa.Extensions;
using AutomationHUB.Engine.Elsa.MessageConsumers;
using AutomationHUB.Engine.Services.Subscribers;
using AutomationHUB.Messaging.Devices;
using AutomationHUB.Messaging.Extensions;
using AutomationHUB.Messaging.Nats.Extensions;
using AutomationHUB.Messaging.Registry;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Configure CORS to allow designer app hosted on a different origin to invoke the APIs.
builder.Services.AddCors(cors => cors
    .AddDefaultPolicy(policy => policy
        .AllowAnyOrigin() // Allow specific origin
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("*")));


//Elsa
builder.Services.AddElsaDependencies(configuration);

//Nats
builder.Services.AddNats(configuration);
builder.Services.AddNatsSubscriber();
//Messaging
builder.Services.AddHostedService<DomainSubscriberHostedService<DeviceMessage>>();
builder.Services.AddHostedService<DomainSubscriberHostedService<RegistryMessage>>();

//Messaging
builder.Services.AddAutomationMessageConsumer<DeviceMessage, DeviceMessageElsaConsumer>();
builder.Services.AddAutomationMessageConsumer<DeviceMessage, DeviceMessageSignalRPublisher>();

builder.Services.AddSingleton<IDeviceConfigurationService, DeviceConfigurationService>();

builder.Services.AddSignalR();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


//Elsa
app.UseElsaDependencies();

app.MapControllers();

app.MapHub<DeviceHub>(SignalRRoutes.DeviceHubPath);

app.Run("https://localhost:5001");
