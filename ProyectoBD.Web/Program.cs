using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProyectoBD.Negocio;
using ProyectoBD.Web.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Infraestructura opcional: misma identidad y claves para todas las réplicas.
string? rutaClaves = Environment.GetEnvironmentVariable("DATA_PROTECTION_KEYS_PATH");
if (!string.IsNullOrWhiteSpace(rutaClaves))
{
    Directory.CreateDirectory(rutaClaves);
    var proteccion = builder.Services.AddDataProtection()
        .SetApplicationName("ProyectoBD.Web")
        .PersistKeysToFileSystem(new DirectoryInfo(rutaClaves));
    string? certificado = Environment.GetEnvironmentVariable("DATA_PROTECTION_CERTIFICATE_PATH");
    if (!string.IsNullOrWhiteSpace(certificado))
        proteccion.ProtectKeysWithCertificate(new System.Security.Cryptography.X509Certificates.X509Certificate2(
            certificado, Environment.GetEnvironmentVariable("DATA_PROTECTION_CERTIFICATE_PASSWORD"),
            System.Security.Cryptography.X509Certificates.X509KeyStorageFlags.EphemeralKeySet));
}

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHealthChecks();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ProyectoBD.Web.Services.ReportesMemoria>();

//Agregar UsuarioNegocio para poder usarlo en los Controllers
builder.Services.AddScoped<UsuarioNegocio>();

builder.Services.AddScoped<RolNegocio>();
builder.Services.AddScoped<ClienteNegocio>();
builder.Services.AddScoped<ProveedorNegocio>();
builder.Services.AddScoped<MarcaNegocio>();
builder.Services.AddScoped<CategoriaNegocio>();
builder.Services.AddScoped<PresentacionNegocio>();
builder.Services.AddScoped<ColorProductoNegocio>();
builder.Services.AddScoped<ProductoNegocio>();
builder.Services.AddScoped<UbicacionNegocio>();
builder.Services.AddScoped<MetodoPagoNegocio>();

//Configurar autenticación por cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // Ruta de inicio de sesión
        options.AccessDeniedPath = "/Account/AccesoDenegado"; // Ruta de acceso denegado
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<IProxyAddressResolver, ProxyAddressResolver>();
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

// Restore the original HTTPS scheme before HSTS, redirects and cookies.
app.UseMiddleware<TrustedProxyForwardingMiddleware>();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Internal Docker/Kubernetes probes use HTTP; public TLS is terminated by the proxy.
app.UseWhen(context => context.Request.Path != "/health", branch => branch.UseHttpsRedirection());
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHealthChecks("/health");

app.Run();
