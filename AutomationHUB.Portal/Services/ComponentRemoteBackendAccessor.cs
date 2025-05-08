using Elsa.Api.Client.Options;
using Elsa.Studio.Contracts;
using Elsa.Studio.Models;
using Elsa.Studio.Options;
using Elsa.Studio.Services;
using Microsoft.Extensions.Options;

namespace AutomationHUB.Portal.Services;

/// <summary>
/// A default implementation of <see cref="IRemoteBackendAccessor"/> that uses the <see cref="BackendOptions"/> to determine the URL of the remote backend.
/// </summary>
public class ComponentRemoteBackendAccessor : IRemoteBackendAccessor
{
    private readonly IOptions<BackendOptions> _options;

    private readonly BackendService _backendService;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultRemoteBackendAccessor"/> class.
    /// </summary>
    public ComponentRemoteBackendAccessor(BackendService backendService, IOptions<BackendOptions> options)
    {
        _options = options;
        _backendService = backendService;
        _backendService.RemoteEndpoint = options.Value.Url.ToString();
        _backendService.ApiKey = Guid.Empty.ToString();
    }

    /// <inheritdoc />
    public RemoteBackend RemoteBackend => new(new(_backendService.RemoteEndpoint));
}