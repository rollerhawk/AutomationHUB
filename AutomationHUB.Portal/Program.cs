using AutomationHUB.Portal;
using AutomationHUB.Portal.Components;
using AutomationHUB.Portal.ElsaStudio.Extensions;
using AutomationHUB.Portal.Services;
using Elsa.Api.Client.HttpMessageHandlers;
using Elsa.Studio.Components;
using Elsa.Studio.Contracts;
using Elsa.Studio.Core.BlazorServer.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Localization.Time.Providers;
using Elsa.Studio.Localization.Time;
using Elsa.Studio.Models;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Workflows.Designer.Extensions;
using Elsa.Studio.Workflows.Extensions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Web;
using AutomationHUB.Portal.HttpMessageHandlers;
using Elsa.Studio.Login.HttpMessageHandlers;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    options.DetailedErrors = true;
});

builder.Services.AddMudServices();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.RootComponents.RegisterCustomElsaStudioElements();
        options.RootComponents.RegisterCustomElement<BackendProvider>("elsa-backend-provider");
        options.RootComponents.RegisterCustomElement<WorkflowDefinitionEditorWrapper>("elsa-workflow-definition-editor");
        options.RootComponents.RegisterCustomElement<WorkflowInstanceViewerWrapper>("elsa-workflow-instance-viewer");
        options.RootComponents.RegisterCustomElement<WorkflowInstanceListWrapper>("elsa-workflow-instance-list");
        options.RootComponents.RegisterCustomElement<WorkflowDefinitionListWrapper>("elsa-workflow-definition-list");
    });

// Register local services.
builder.Services.AddSingleton<BackendService>();

// Register the modules.
var backendApiConfig = new BackendApiConfig
{
    ConfigureBackendOptions = options => configuration.GetSection("Backend").Bind(options),
    ConfigureHttpClientBuilder = clientOptions =>
    {
        clientOptions.AuthenticationHandler = typeof(AuthHttpMessageHandler);
    },
};

builder.Services.AddCore();
builder.Services.AddShell();
builder.Services.AddRemoteBackend(backendApiConfig);
builder.Services.Replace(ServiceDescriptor.Scoped<IRemoteBackendAccessor, ComponentRemoteBackendAccessor>());
builder.Services.AddWorkflowsModule();
builder.Services.AddScoped<ITimeZoneProvider, LocalTimeZoneProvider>();

// Build the application.
var app = builder.Build();

app.UseHttpsRedirection();


app.UseStaticFiles();
app.UseAntiforgery();
app.UseAntiforgery();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// Run the application.
app.Run();
