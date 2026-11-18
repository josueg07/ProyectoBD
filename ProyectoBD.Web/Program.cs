using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Authentication.Cookies;
using ProyectoBD.Negocio;

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
builder.Services.Configure<Microsoft.AspNetCore.Builder.ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedFor
        | Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto;
    foreach (string red in (Environment.GetEnvironmentVariable("TRUSTED_PROXY_NETWORKS") ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    {
        string[] partes = red.Split('/');
        options.KnownNetworks.Add(new Microsoft.AspNetCore.HttpOverrides.IPNetwork(System.Net.IPAddress.Parse(partes[0]), int.Parse(partes[1])));
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseForwardedHeaders();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHealthChecks("/health");

app.Run();
