using AutomationHUB.Engine.Elsa.Activities;
using AutomationHUB.Engine.Elsa.Providers;
using Elsa.EntityFrameworkCore.Extensions;
using Elsa.EntityFrameworkCore.Modules.Management;
using Elsa.EntityFrameworkCore.Modules.Runtime;
using Elsa.Extensions;
using Elsa.Workflows;
using Microsoft.AspNetCore.Builder;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AutomationHUB.Engine.Elsa.Extensions;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds necessary dependencies for Elsa to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddElsaDependencies(this IServiceCollection services, IConfiguration configuration)
    {      
        
        services.AddElsa(elsa =>
        {
            // Configure Management layer to use EF Core.
            elsa.UseWorkflowManagement(management => management.UseEntityFrameworkCore(ef => ef.UseSqlite()));

            // Configure Runtime layer to use EF Core.
            elsa.UseWorkflowRuntime(runtime => runtime.UseEntityFrameworkCore(ef => ef.UseSqlite()));

            // Default Identity features for authentication/authorization.
            elsa.UseIdentity(identity =>
            {
                identity.TokenOptions = options => options.SigningKey = "72f67f4c1cf35c3f41ac79af42ee13c8b5f487bddccfa717c2b68013f556fd33c7b359c1518064243c44c55935b54cced61a37e0eb74a2e3e8f6d02c9810e58e278377ca230d317fe4759fc93ba40e305370cb498403e95c0f40edbb6f22752b5378ff9929fffe7b1ff2545341912fccc406b11060f57e19ff8ea97c6b412d0b7ccc5160fe5b2bf0663f5eb36952668ee913e85c5e5b31c4cb0f6ea5d4844878efc2a02c6b0bdec57cca70247700e7aac694a959cf776e964b8307db0063ff1c220babfb32a9829afe56de1f457a7f19e7fcb5650c9ca3e30e437b28e242f42d17061c69112274007ea1525e597c07f510e143e9ea0683b57f7713dfa325bf77"; // This key needs to be at least 256 bits long.
                identity.UseAdminUserProvider();
            });

            // Configure ASP.NET authentication/authorization.
            elsa.UseDefaultAuthentication(auth => auth.UseAdminApiKey());

            // Expose Elsa API endpoints.
            elsa.UseWorkflowsApi();

            // Setup a SignalR hub for real-time updates from the server.
            elsa.UseRealTimeWorkflows();

            // Enable C# workflow expressions.
            elsa.UseCSharp();

            // Enable HTTP activities.
            elsa.UseHttp(http => http.ConfigureHttpOptions = options => configuration.GetSection("Http").Bind(options));

            // Register custom activities from the application, if any.
            //elsa.AddActivitiesFrom<IAutomationMessageEvent>();            

            //// Register custom workflows from the application, if any.
            //elsa.AddWorkflowsFrom<IAutomationMessageEvent>();
        });

        // 2) Then remove the default provider…
        services.RemoveAll<IActivityProvider>();

        // 3) …and add yours
        services.AddSingleton<IActivityProvider, AutomationDeviceActivityProvider>();

        return services;
    }

    public static IApplicationBuilder UseElsaDependencies(this IApplicationBuilder app)
    {
        // Configure CORS to allow designer app hosted on a different origin to invoke the APIs.
        app.UseHttpsRedirection();        
        app.UseCors();
        app.UseRouting(); // Required for SignalR.
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseWorkflowsApi(); // Use Elsa API endpoints.
        app.UseWorkflows(); // Use Elsa middleware to handle HTTP requests mapped to HTTP Endpoint activities.
        app.UseWorkflowsSignalRHubs(); // Optional SignalR integration. Elsa Studio uses SignalR to receive real-time updates from the server.
        return app;
    }
}
