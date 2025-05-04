using Elsa.Studio.Core.BlazorServer.Extensions;
using Elsa.Studio.Dashboard.Extensions;
using Elsa.Studio.Extensions;
using Elsa.Studio.Login.BlazorServer.Extensions;
using Elsa.Studio.Login.Extensions;
using Elsa.Studio.Login.HttpMessageHandlers;
using Elsa.Studio.Models;
using Elsa.Studio.Shell.Extensions;
using Elsa.Studio.Workflows.Designer.Extensions;
using Elsa.Studio.Workflows.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationHUB.Portal.ElsaStudio.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddElsaStudio(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor(options =>
            {
                // Register the root components.    
                options.RootComponents.RegisterCustomElsaStudioElements();
            });

            // Register shell services and modules.
            services.AddCore();
            services.AddShell();
            services.AddRemoteBackend(new BackendApiConfig()
            {
                ConfigureHttpClientBuilder = elsaClient => elsaClient.AuthenticationHandler = typeof(AuthenticatingApiHttpMessageHandler),
                ConfigureBackendOptions = options => configuration.GetSection("Backend").Bind(options)
            });

            services.UseElsaIdentity();
            services.AddLoginModule();
            services.AddDashboardModule();
            services.AddWorkflowsModule();

            // Configure SignalR.
            services.AddSignalR(options =>
            {
                // Set MaximumReceiveMessageSize to handle large workflows.
                options.MaximumReceiveMessageSize = 5 * 1024 * 1000; // 5MB
            });
            return services;
        }

        public static IApplicationBuilder UseElsaStudio(this IApplicationBuilder app)
        {
            // Configure CORS to allow designer app hosted on a different origin to invoke the APIs.
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            return app;
        }
    }
}
