using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ProyectoBD.Web.Infrastructure;

using var logs = LoggerFactory.Create(_ => { });
var clock = new TestClock();
var dns = new TestDns { Addresses = [IPAddress.Parse("10.0.1.7")] };
var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
{
    ["TRUSTED_PROXY_HOSTS"] = "dokploy-traefik",
    ["TRUSTED_PROXY_NETWORKS"] = "10.0.1.0/24"
}).Build();
var middleware = new TrustedProxyForwardingMiddleware(context =>
{
    context.Response.Headers.Location = $"{context.Request.Scheme}://{context.Request.Host}/Account/Login";
    return Task.CompletedTask;
}, logs, configuration, dns, clock);

async Task Check(string name, string peer, string expected, string proto = "https")
{
    var context = new DefaultHttpContext();
    context.Connection.RemoteIpAddress = IPAddress.Parse(peer);
    context.Request.Scheme = "http";
    context.Request.Path = "/";
    context.Request.Host = new HostString("proyectobd.duckdns.org");
    context.Request.Headers["X-Forwarded-Proto"] = proto;
    context.Request.Headers["X-Forwarded-For"] = "203.0.113.20";
    context.Request.Headers["X-Forwarded-Host"] = "evil.invalid";
    await middleware.InvokeAsync(context);
    if (context.Request.Scheme != expected || context.Request.Host.Host != "proyectobd.duckdns.org" ||
        (expected == "http" && (context.Response.StatusCode is not (400 or 503) || context.Response.Headers.ContainsKey("Location"))))
        throw new Exception($"FAIL {name}: {context.Response.Headers.Location}");
    Console.WriteLine($"PASS {name}");
}

await Check("exact trusted proxy restores HTTPS", "10.0.1.7", "https");
await Check("other container in the same subnet cannot spoof HTTPS", "10.0.1.3", "http");
await Check("external peer cannot spoof HTTPS", "203.0.113.40", "http");
await Check("loopback is not implicitly trusted in DNS mode", "127.0.0.1", "http");
await Check("IPv4-mapped IPv6 peer works", "::ffff:10.0.1.7", "https");
await Check("only rightmost hop is consumed", "10.0.1.7", "https", "http, https");
dns.Addresses = [IPAddress.Parse("10.0.1.22")];
clock.Advance();
await Check("new Traefik IP is accepted after DNS refresh", "10.0.1.22", "https");
await Check("old Traefik IP loses trust", "10.0.1.7", "http");
dns.Addresses = [IPAddress.Parse("198.51.100.5")];
clock.Advance();
await Check("DNS result outside allowed CIDR rejected", "198.51.100.5", "http");
dns.Fail = true;
clock.Advance();
await Check("DNS failure fails closed", "10.0.1.22", "http");
dns.Fail = false;
dns.Addresses = [IPAddress.Parse("10.0.1.22")];
clock.Advance();
await Check("recovers after DNS failure", "10.0.1.22", "https");

foreach (string bad in new[] { "0.0.0.0/0", "::/0", "10.0.1.0/33", "invalid" })
{
    configuration["TRUSTED_PROXY_NETWORKS"] = bad;
    try
    {
        _ = new TrustedProxyForwardingMiddleware(_ => Task.CompletedTask, logs, configuration, dns, clock);
        throw new Exception($"FAIL invalid CIDR accepted: {bad}");
    }
    catch (InvalidOperationException) { Console.WriteLine($"PASS invalid CIDR rejected: {bad}"); }
}
Console.WriteLine("All proxy infrastructure checks passed; no database accessed.");

sealed class TestDns : IProxyAddressResolver
{
    public IPAddress[] Addresses { get; set; } = [];
    public bool Fail { get; set; }
    public Task<IPAddress[]> ResolveAsync(string host) => Fail
        ? Task.FromException<IPAddress[]>(new SocketException()) : Task.FromResult(Addresses);
}

sealed class TestClock : TimeProvider
{
    private DateTimeOffset now = DateTimeOffset.UtcNow;
    public override DateTimeOffset GetUtcNow() => now;
    public void Advance() => now = now.AddSeconds(31);
}
