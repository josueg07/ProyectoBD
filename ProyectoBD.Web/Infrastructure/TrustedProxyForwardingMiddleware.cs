using System.Net;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using ProxyNetwork = Microsoft.AspNetCore.HttpOverrides.IPNetwork;

namespace ProyectoBD.Web.Infrastructure;

public interface IProxyAddressResolver
{
    Task<IPAddress[]> ResolveAsync(string host);
}

public sealed class ProxyAddressResolver : IProxyAddressResolver
{
    public async Task<IPAddress[]> ResolveAsync(string host) =>
        await Dns.GetHostAddressesAsync(host).WaitAsync(TimeSpan.FromSeconds(3));
}

// The subnet constrains DNS answers; it does not grant trust to the whole network.
public sealed class TrustedProxyForwardingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILoggerFactory loggerFactory;
    private readonly ILogger logger;
    private readonly IProxyAddressResolver resolver;
    private readonly TimeProvider clock;
    private readonly string[] hosts;
    private readonly ProxyNetwork[] networks;
    private readonly SemaphoreSlim refreshLock = new(1, 1);
    private Snapshot snapshot = new(DateTimeOffset.MinValue, [], null);
    private readonly ForwardedHeadersMiddleware legacy;

    public TrustedProxyForwardingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory,
        IConfiguration configuration, IProxyAddressResolver resolver, TimeProvider clock)
    {
        this.next = next;
        this.loggerFactory = loggerFactory;
        logger = loggerFactory.CreateLogger<TrustedProxyForwardingMiddleware>();
        this.resolver = resolver;
        this.clock = clock;
        hosts = Split(configuration["TRUSTED_PROXY_HOSTS"]);
        networks = Split(configuration["TRUSTED_PROXY_NETWORKS"]).Select(ParseNetwork).ToArray();
        if (hosts.Any(h => h.Contains('.') || Uri.CheckHostName(h) != UriHostNameType.Dns))
            throw new InvalidOperationException("TRUSTED_PROXY_HOSTS requires internal Docker DNS names, without domain or IP.");
        if (hosts.Length > 0 && networks.Length == 0)
            throw new InvalidOperationException("TRUSTED_PROXY_HOSTS requires TRUSTED_PROXY_NETWORKS to constrain DNS answers.");

        var options = CreateOptions();
        // Preserve explicit CIDRs used by the existing local/Kubernetes deployment.
        if (hosts.Length == 0)
            foreach (var network in networks) options.KnownNetworks.Add(network);
        legacy = new ForwardedHeadersMiddleware(next, loggerFactory, Options.Create(options));
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (hosts.Length == 0)
        {
            await legacy.Invoke(context);
            return;
        }

        var current = Volatile.Read(ref snapshot);
        if (clock.GetUtcNow() >= current.Expires)
        {
            await refreshLock.WaitAsync(context.RequestAborted);
            try
            {
                current = Volatile.Read(ref snapshot);
                if (clock.GetUtcNow() >= current.Expires)
                {
                    IPAddress[] addresses = [];
                    try
                    {
                        var answers = await Task.WhenAll(hosts.Select(resolver.ResolveAsync));
                        addresses = answers.SelectMany(a => a).Select(Normalize)
                            .Where(a => networks.Any(n => n.Contains(a))).Distinct().ToArray();
                    }
                    catch (Exception error) when (error is System.Net.Sockets.SocketException or TimeoutException)
                    {
                        logger.LogWarning("Proxy DNS failed; forwarded headers will not be trusted until the next lookup.");
                    }
                    var options = CreateOptions();
                    foreach (var address in addresses) options.KnownProxies.Add(address);
                    current = new(clock.GetUtcNow().AddSeconds(30), addresses,
                        new ForwardedHeadersMiddleware(next, loggerFactory, Options.Create(options)));
                    Volatile.Write(ref snapshot, current);
                }
            }
            finally { refreshLock.Release(); }
        }

        IPAddress? remote = context.Connection.RemoteIpAddress;
        if (remote is not null && current.Addresses.Contains(Normalize(remote)))
            await current.Middleware!.Invoke(context);
        else if (context.Request.Path == "/health" &&
                 (HttpMethods.IsGet(context.Request.Method) || HttpMethods.IsHead(context.Request.Method)))
            await next(context); // Container probes need no forwarded headers.
        else
        {
            // Never produce an HTTP authentication redirect while proxy DNS is
            // unavailable, or when another container attempts to impersonate it.
            context.Response.StatusCode = current.Addresses.Length == 0
                ? StatusCodes.Status503ServiceUnavailable : StatusCodes.Status400BadRequest;
        }
    }

    private static ForwardedHeadersOptions CreateOptions() => new()
    {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        ForwardLimit = 1
    };

    private static string[] Split(string? value) => (value ?? "").Split(',',
        StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    private static IPAddress Normalize(IPAddress value) => value.IsIPv4MappedToIPv6 ? value.MapToIPv4() : value;

    private static ProxyNetwork ParseNetwork(string value)
    {
        string[] parts = value.Split('/');
        if (parts.Length != 2 || !IPAddress.TryParse(parts[0], out var address) ||
            !int.TryParse(parts[1], out int prefix) || prefix <= 0 ||
            prefix > (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork ? 32 : 128))
            throw new InvalidOperationException("TRUSTED_PROXY_NETWORKS requires explicit non-universal CIDRs.");
        return new ProxyNetwork(address, prefix);
    }

    private sealed record Snapshot(DateTimeOffset Expires, IPAddress[] Addresses, ForwardedHeadersMiddleware? Middleware);
}
