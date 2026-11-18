using System.Net;
using System.Text.RegularExpressions;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;

internal static partial class Program
{
    private static readonly string[] Modulos = { "Clientes", "Proveedores", "Marcas", "Categorias", "Presentaciones", "Colores", "Productos", "Variantes" };
    private static readonly string Prefijo = "wv" + Guid.NewGuid().ToString("N")[..10];
    private static readonly UsuarioNegocio Usuarios = new();
    private static readonly ClienteNegocio Clientes = new();
    private static readonly ProveedorNegocio Proveedores = new();
    private static readonly MarcaNegocio Marcas = new();
    private static readonly CategoriaNegocio Categorias = new();
    private static readonly PresentacionNegocio Presentaciones = new();
    private static readonly ColorProductoNegocio Colores = new();
    private static readonly ProductoNegocio Productos = new();
    private static string servidor = "http://127.0.0.1:5187";
    private static int actor;

    private static void Exigir(bool condicion, string mensaje)
    {
        if (!condicion) throw new InvalidOperationException(mensaje);
    }
    private static HttpClient ClienteHttp()
    {
        return new HttpClient(new HttpClientHandler { AllowAutoRedirect = false, CookieContainer = new CookieContainer() })
        {
            BaseAddress = new Uri(servidor), Timeout = TimeSpan.FromSeconds(60)
        };
    }
    private static async Task<string> Pagina(HttpClient cliente, string ruta)
    {
        using var respuesta = await cliente.GetAsync(ruta);
        string html = await respuesta.Content.ReadAsStringAsync();
        Exigir(respuesta.StatusCode == HttpStatusCode.OK, "GET " + ruta + ": " + respuesta.StatusCode);
        Exigir(!html.Contains("No se pudo cargar el listado") && !html.Contains("No se pudieron cargar"), "Falló carga de datos en " + ruta);
        return html;
    }
    private static string Token(string html)
    {
        foreach (Match entrada in Regex.Matches(html, "<input\\b[^>]*>", RegexOptions.IgnoreCase))
        {
            if (!entrada.Value.Contains("name=\"__RequestVerificationToken\"")) continue;
            return WebUtility.HtmlDecode(Regex.Match(entrada.Value, "value=\"([^\"]+)\"").Groups[1].Value);
        }
        throw new InvalidOperationException("No se encontró token antiforgery.");
    }
    private static async Task<HttpResponseMessage> Enviar(HttpClient cliente, string ruta, string formulario,
        Dictionary<string, string> campos, bool conToken = true)
    {
        if (conToken) campos["__RequestVerificationToken"] = Token(await Pagina(cliente, formulario));
        return await cliente.PostAsync(ruta, new FormUrlEncodedContent(campos));
    }
    private static async Task Entrar(HttpClient cliente, string usuario, string password)
    {
        using var respuesta = await Enviar(cliente, "/Account/Login", "/Account/Login", new() { ["Usuario"] = usuario, ["Password"] = password });
        Exigir(respuesta.StatusCode == HttpStatusCode.Redirect && new[] { "/", "/Home", "/Home/Index" }.Contains(respuesta.Headers.Location?.ToString()), "Falló login de prueba.");
        await Pagina(cliente, "/Home/Index");
    }
    private static async Task Redireccion(HttpClient cliente, string ruta, string formulario, Dictionary<string, string> campos)
    {
        using var respuesta = await Enviar(cliente, ruta, formulario, campos);
        if (respuesta.StatusCode != HttpStatusCode.Redirect)
        {
            string html = await respuesta.Content.ReadAsStringAsync();
            string errores = string.Join(" | ", Regex.Matches(html, "<(?:li|span)[^>]*>([^<]+)</(?:li|span)>")
                .Select(m => WebUtility.HtmlDecode(m.Groups[1].Value)).TakeLast(12));
            throw new InvalidOperationException(ruta + " no redirige: " + respuesta.StatusCode + " | " + errores);
        }
        await Pagina(cliente, respuesta.Headers.Location!.ToString());
    }
    private static async Task Invalido(HttpClient cliente, string modulo, Dictionary<string, string> campos)
    {
        using var respuesta = await Enviar(cliente, "/" + modulo + "/Crear", "/" + modulo + "/Crear", campos);
        string html = await respuesta.Content.ReadAsStringAsync();
        Exigir(respuesta.StatusCode == HttpStatusCode.OK && html.Contains("field-validation-error"), "No validó entradas de " + modulo);
        if (modulo == "Productos" || modulo == "Variantes")
            Exigir(Regex.Matches(html, "<option\\b").Count > 3, "No recargó los catálogos de " + modulo);
    }
    private static async Task Eliminar(HttpClient cliente, string modulo, int id)
    {
        await Redireccion(cliente, $"/{modulo}/Eliminar/{id}", $"/{modulo}/Index", new());
    }
    private static async Task RechazarRelacion(HttpClient cliente, string modulo, int id, Func<bool> existe)
    {
        Exigir(existe(), "No existe el registro relacionado para probar " + modulo);
        using var respuesta = await Enviar(cliente, $"/{modulo}/Eliminar/{id}", $"/{modulo}/Index", new());
        Exigir(respuesta.StatusCode == HttpStatusCode.Redirect, "La FK causó error HTTP en " + modulo);
        string html = WebUtility.HtmlDecode(await Pagina(cliente, respuesta.Headers.Location!.ToString()));
        Exigir(html.Contains("No se puede eliminar porque el registro está siendo utilizado por otras operaciones.") && existe(), "No protegió la relación de " + modulo);
        Exigir(!html.Contains("SqlException") && !html.Contains("stack trace", StringComparison.OrdinalIgnoreCase), "Expuso detalle técnico de FK.");
        Console.WriteLine("OK FK protegida y mensaje entendible: " + modulo);
    }
    private static async Task Seguridad(HttpClient anonimo, HttpClient gerente, HttpClient cajero, HttpClient digitador)
    {
        foreach (string modulo in Modulos)
        {
            using (var respuesta = await anonimo.GetAsync($"/{modulo}/Index"))
                Exigir(respuesta.StatusCode == HttpStatusCode.Redirect && respuesta.Headers.Location!.ToString().Contains("/Account/Login"), "Acceso anónimo a " + modulo);
            foreach (var cliente in new[] { gerente, cajero })
            {
                foreach (string accion in new[] { "Index", "Crear", "Editar/1" })
                {
                    using var respuesta = await cliente.GetAsync($"/{modulo}/{accion}");
                    Exigir(respuesta.StatusCode == HttpStatusCode.Redirect && respuesta.Headers.Location!.ToString().Contains("/Account/AccesoDenegado"), "GET no restringido: " + modulo + "/" + accion);
                }
                foreach (string accion in new[] { "Crear", "Editar/1", "Eliminar/1" })
                {
                    using var respuesta = await cliente.PostAsync($"/{modulo}/{accion}", new FormUrlEncodedContent(new Dictionary<string, string>()));
                    Exigir(respuesta.StatusCode == HttpStatusCode.Redirect && respuesta.Headers.Location!.ToString().Contains("/Account/AccesoDenegado"), "POST no restringido: " + modulo + "/" + accion);
                }
            }
            Exigir(WebUtility.HtmlDecode(await Pagina(digitador, $"/{modulo}/Index")).Contains("Eliminar"), "Listado no funcional: " + modulo);
            await Pagina(digitador, $"/{modulo}/Crear");
            using (var respuesta = await digitador.GetAsync($"/{modulo}/Eliminar/1"))
                Exigir(respuesta.StatusCode == HttpStatusCode.MethodNotAllowed, "Permite eliminar mediante GET: " + modulo);
            foreach (string accion in new[] { "Crear", "Editar/1", "Eliminar/1" })
            {
                using var respuesta = await Enviar(digitador, $"/{modulo}/{accion}", "", new(), false);
                Exigir(respuesta.StatusCode == HttpStatusCode.BadRequest, "No exige antiforgery: " + modulo + "/" + accion);
            }
            using (var respuesta = await digitador.GetAsync($"/{modulo}/Editar/{int.MaxValue}"))
                Exigir(respuesta.StatusCode == HttpStatusCode.NotFound, "No maneja registro inexistente: " + modulo);
            Console.WriteLine("OK listado, formularios, permisos GET/POST, antiforgery y eliminación sólo POST: " + modulo);
        }
        string inicio = await Pagina(digitador, "/Home/Index");
        foreach (string modulo in Modulos) Exigir(inicio.Contains("href=\"/" + modulo + "\""), "Falta enlace de menú: " + modulo);
        foreach (var cliente in new[] { gerente, cajero })
        {
            string html = await Pagina(cliente, "/Home/Index");
            Exigir(!html.Contains("href=\"/Productos\""), "Muestra mantenimientos a otro rol.");
        }
        string denegado = WebUtility.HtmlDecode(await Pagina(gerente, "/Account/AccesoDenegado"));
        Exigir(denegado.Contains("Acceso denegado", StringComparison.OrdinalIgnoreCase), "La ruta de acceso denegado no tiene vista.");
        await Pagina(gerente, "/Usuarios/Index");
        await Pagina(gerente, "/Roles/Index");
        Console.WriteLine("OK menú por rol, Usuarios/Roles existentes y acceso denegado.");
    }

    private static async Task Crud(HttpClient cliente, UsuarioSesion gerente, UsuarioSesion digitador)
    {
        await Invalido(cliente, "Clientes", new() { ["NombreRazonSocial"] = "", ["TipoCliente"] = "Otro" });
        await Invalido(cliente, "Proveedores", new() { ["Nombre_Proveedor"] = "" });
        await Invalido(cliente, "Marcas", new() { ["NombreMarca"] = "" });
        await Invalido(cliente, "Categorias", new() { ["NombreCategoria"] = "", ["Descripcion"] = "" });
        await Invalido(cliente, "Presentaciones", new() { ["NombrePresentacion"] = "", ["UnidadMedida"] = "" });
        await Invalido(cliente, "Colores", new() { ["NombreColor"] = new string('x', 46) });
        await Invalido(cliente, "Productos", new() { ["NombreProducto"] = "", ["Descripcion"] = "", ["ID_CategoriaP"] = "2147483647", ["ID_Marca"] = "2147483647" });
        await Invalido(cliente, "Variantes", new() { ["ID_Producto"] = "2147483647", ["ID_Presentacion"] = "2147483647", ["ID_Color"] = "2147483647", ["PrecioVenta"] = "-1" });
        Console.WriteLine("OK validaciones obligatorias, longitudes y FK de los ocho módulos; catálogos recargados.");

        await Redireccion(cliente, "/Clientes/Crear", "/Clientes/Crear", new() { ["TipoCliente"] = "Particular", ["NombreRazonSocial"] = Prefijo + " cliente" });
        int clienteId = Clientes.Listar().Single(c => c.NombreRazonSocial == Prefijo + " cliente").ID_Cliente;
        await Redireccion(cliente, $"/Clientes/Editar/{clienteId}", $"/Clientes/Editar/{clienteId}", new() { ["ID_Cliente"] = clienteId.ToString(), ["TipoCliente"] = "Empresa", ["NombreRazonSocial"] = Prefijo + " editado" });
        Exigir(Clientes.Listar().Single(c => c.ID_Cliente == clienteId).TipoCliente == "Empresa", "No editó cliente.");

        await Redireccion(cliente, "/Proveedores/Crear", "/Proveedores/Crear", new() { ["Nombre_Proveedor"] = Prefijo + " proveedor" });
        int proveedorId = Proveedores.Listar().Single(p => p.Nombre_Proveedor == Prefijo + " proveedor").ID_Proveedor;
        await Redireccion(cliente, $"/Proveedores/Editar/{proveedorId}", $"/Proveedores/Editar/{proveedorId}", new() { ["ID_Proveedor"] = proveedorId.ToString(), ["Nombre_Proveedor"] = Prefijo + " proveedor editado" });

        await Redireccion(cliente, "/Marcas/Crear", "/Marcas/Crear", new() { ["NombreMarca"] = Prefijo + " marca" });
        int marcaId = Marcas.Listar().Single(m => m.NombreMarca == Prefijo + " marca").ID_Marca;
        await Redireccion(cliente, $"/Marcas/Editar/{marcaId}", $"/Marcas/Editar/{marcaId}", new() { ["ID_Marca"] = marcaId.ToString(), ["NombreMarca"] = Prefijo + " marca editada" });

        await Redireccion(cliente, "/Categorias/Crear", "/Categorias/Crear", new() { ["NombreCategoria"] = Prefijo + " categoria", ["Descripcion"] = "Prueba web" });
        int categoriaId = Categorias.Listar().Single(c => c.NombreCategoria == Prefijo + " categoria").ID_Categoria;
        await Redireccion(cliente, $"/Categorias/Editar/{categoriaId}", $"/Categorias/Editar/{categoriaId}", new() { ["ID_Categoria"] = categoriaId.ToString(), ["NombreCategoria"] = Prefijo + " categoria editada", ["Descripcion"] = "Editada desde HTTP" });

        await Redireccion(cliente, "/Presentaciones/Crear", "/Presentaciones/Crear", new() { ["NombrePresentacion"] = Prefijo + " presentacion", ["UnidadMedida"] = "unidad" });
        int presentacionId = Presentaciones.Listar().Single(p => p.NombrePresentacion == Prefijo + " presentacion").ID_Presentacion;
        await Redireccion(cliente, $"/Presentaciones/Editar/{presentacionId}", $"/Presentaciones/Editar/{presentacionId}", new() { ["ID_Presentacion"] = presentacionId.ToString(), ["NombrePresentacion"] = Prefijo + " presentacion editada", ["UnidadMedida"] = "caja" });

        await Redireccion(cliente, "/Colores/Crear", "/Colores/Crear", new() { ["NombreColor"] = Prefijo + " color" });
        int colorId = Colores.Listar().Single(c => c.NombreColor == Prefijo + " color").ID_Color;
        await Redireccion(cliente, $"/Colores/Editar/{colorId}", $"/Colores/Editar/{colorId}", new() { ["ID_Color"] = colorId.ToString(), ["NombreColor"] = Prefijo + " color editado" });

        await Redireccion(cliente, "/Productos/Crear", "/Productos/Crear", new() { ["NombreProducto"] = Prefijo + " producto", ["Descripcion"] = "Prueba web", ["ID_CategoriaP"] = categoriaId.ToString(), ["ID_Marca"] = marcaId.ToString() });
        int productoId = Productos.Listar().Single(p => p.NombreProducto == Prefijo + " producto").ID_Producto;
        await Redireccion(cliente, $"/Productos/Editar/{productoId}", $"/Productos/Editar/{productoId}", new() { ["ID_Producto"] = productoId.ToString(), ["NombreProducto"] = Prefijo + " producto editado", ["Descripcion"] = "Editado desde HTTP", ["ID_CategoriaP"] = categoriaId.ToString(), ["ID_Marca"] = marcaId.ToString() });
        string productosHtml = WebUtility.HtmlDecode(await Pagina(cliente, "/Productos/Index"));
        Exigir(productosHtml.Contains(Prefijo + " categoria editada") && productosHtml.Contains(Prefijo + " marca editada"), "El listado de productos no muestra nombres de FK.");

        await Redireccion(cliente, "/Variantes/Crear", "/Variantes/Crear?producto=" + productoId, new() { ["ID_Producto"] = productoId.ToString(), ["ID_Presentacion"] = presentacionId.ToString(), ["ID_Color"] = "", ["PrecioVenta"] = "12.50" });
        int varianteId = Productos.ListarVariantes(productoId).Single().ID_ProductoV;
        Exigir(Productos.ListarVariantes(productoId).Single().ID_Color == null, "No conserva color opcional NULL.");
        await Redireccion(cliente, $"/Variantes/Editar/{varianteId}", $"/Variantes/Editar/{varianteId}", new() { ["ID_ProductoV"] = varianteId.ToString(), ["ID_Producto"] = productoId.ToString(), ["ID_Presentacion"] = presentacionId.ToString(), ["ID_Color"] = colorId.ToString(), ["PrecioVenta"] = "15.75" });
        ProductoVariante actual = Productos.ListarVariantes(productoId).Single();
        Exigir(actual.PrecioVenta == 15.75m && actual.ID_Color == colorId, "No editó la variante.");
        string variantesHtml = WebUtility.HtmlDecode(await Pagina(cliente, "/Variantes/Index?producto=" + productoId));
        Exigir(variantesHtml.Contains(Prefijo + " producto editado") && variantesHtml.Contains(Prefijo + " presentacion editada") && variantesHtml.Contains(Prefijo + " color editado") && Regex.IsMatch(variantesHtml, "Q15[.,]75"), "Variantes no muestra las descripciones/precio.");
        using (var respuesta = await Enviar(cliente, $"/Variantes/Editar/{varianteId}", $"/Variantes/Editar/{varianteId}", new() { ["ID_ProductoV"] = varianteId.ToString(), ["ID_Producto"] = "1", ["ID_Presentacion"] = presentacionId.ToString(), ["PrecioVenta"] = "8" }))
            Exigir(respuesta.StatusCode == HttpStatusCode.BadRequest, "Permite reasignar la variante a otro producto.");
        using (var respuesta = await Enviar(cliente, "/Variantes/Crear", "/Variantes/Crear", new() { ["ID_Producto"] = productoId.ToString(), ["ID_Presentacion"] = presentacionId.ToString(), ["PrecioVenta"] = "1.234" }))
            Exigir(respuesta.StatusCode == HttpStatusCode.OK && WebUtility.HtmlDecode(await respuesta.Content.ReadAsStringAsync()).Contains("dos decimales"), "No conserva validación de precio de Negocio.");
        await RechazarRelacion(cliente, "Productos", productoId, () => Productos.Listar().Any(p => p.ID_Producto == productoId));
        await RechazarRelacion(cliente, "Marcas", marcaId, () => Marcas.Listar().Any(m => m.ID_Marca == marcaId));
        await RechazarRelacion(cliente, "Categorias", categoriaId, () => Categorias.Listar().Any(c => c.ID_Categoria == categoriaId));
        await RechazarRelacion(cliente, "Presentaciones", presentacionId, () => Presentaciones.Listar().Any(p => p.ID_Presentacion == presentacionId));
        await RechazarRelacion(cliente, "Colores", colorId, () => Colores.Listar().Any(c => c.ID_Color == colorId));

        var procesoGerente = new ProcesoNegocio(gerente);
        string clienteUtilizado = (string)procesoGerente.Facturas().Rows[0]["Cliente"];
        int clienteRelacionado = Clientes.Listar().First(c => c.NombreRazonSocial == clienteUtilizado).ID_Cliente;
        await RechazarRelacion(cliente, "Clientes", clienteRelacionado, () => Clientes.Listar().Any(c => c.ID_Cliente == clienteRelacionado));
        string proveedorUtilizado = (string)new ProcesoNegocio(digitador).Ordenes().Rows[0]["Nombre_Proveedor"];
        int proveedorRelacionado = Proveedores.Listar().First(p => p.Nombre_Proveedor == proveedorUtilizado).ID_Proveedor;
        await RechazarRelacion(cliente, "Proveedores", proveedorRelacionado, () => Proveedores.Listar().Any(p => p.ID_Proveedor == proveedorRelacionado));
        var lote = procesoGerente.Inventario().Rows[0];
        int productoLote = Productos.Listar().First(p => p.NombreProducto == (string)lote["NombreProducto"]).ID_Producto;
        int presentacionLote = Presentaciones.Listar().First(p => p.NombrePresentacion == (string)lote["NombrePresentacion"]).ID_Presentacion;
        int? colorLote = lote.IsNull("NombreColor") ? null : Colores.Listar().First(c => c.NombreColor == (string)lote["NombreColor"]).ID_Color;
        int varianteRelacionada = Productos.ListarVariantes(productoLote).First(v => v.ID_Presentacion == presentacionLote && v.ID_Color == colorLote).ID_ProductoV;
        await RechazarRelacion(cliente, "Variantes", varianteRelacionada, () => Productos.ListarVariantes().Any(v => v.ID_ProductoV == varianteRelacionada));
        Console.WriteLine("OK crear y editar los ocho módulos; variantes con/sin color, precio, filtro, nombres de FK y conservación de producto.");

        await Eliminar(cliente, "Variantes", varianteId);
        await Eliminar(cliente, "Productos", productoId);
        await Eliminar(cliente, "Colores", colorId);
        await Eliminar(cliente, "Presentaciones", presentacionId);
        await Eliminar(cliente, "Categorias", categoriaId);
        await Eliminar(cliente, "Marcas", marcaId);
        await Eliminar(cliente, "Proveedores", proveedorId);
        await Eliminar(cliente, "Clientes", clienteId);
        Exigir(!Productos.ListarVariantes().Any(v => v.ID_ProductoV == varianteId), "No eliminó variante temporal.");
        Exigir(!Productos.Listar().Any(p => p.ID_Producto == productoId) && !Clientes.Listar().Any(c => c.ID_Cliente == clienteId), "No eliminó datos temporales.");
        Console.WriteLine("OK eliminación por POST de los ocho registros temporales.");
    }

    private static void Limpiar(UsuarioSesion gerente)
    {
        var productosPrueba = Productos.Listar().Where(p => p.NombreProducto.StartsWith(Prefijo)).ToList();
        foreach (var p in productosPrueba)
        {
            foreach (var v in Productos.ListarVariantes(p.ID_Producto)) Productos.EliminarVariante(v.ID_ProductoV);
            Productos.Eliminar(p.ID_Producto);
        }
        foreach (var c in Colores.Listar().Where(c => c.NombreColor.StartsWith(Prefijo))) Colores.Eliminar(c.ID_Color);
        foreach (var p in Presentaciones.Listar().Where(p => p.NombrePresentacion.StartsWith(Prefijo))) Presentaciones.Eliminar(p.ID_Presentacion);
        foreach (var c in Categorias.Listar().Where(c => c.NombreCategoria.StartsWith(Prefijo))) Categorias.Eliminar(c.ID_Categoria);
        foreach (var m in Marcas.Listar().Where(m => m.NombreMarca.StartsWith(Prefijo))) Marcas.Eliminar(m.ID_Marca);
        foreach (var p in Proveedores.Listar().Where(p => p.Nombre_Proveedor.StartsWith(Prefijo))) Proveedores.Eliminar(p.ID_Proveedor);
        foreach (var c in Clientes.Listar().Where(c => c.NombreRazonSocial.StartsWith(Prefijo))) Clientes.Eliminar(c.ID_Cliente);
        if (actor > 0) Usuarios.Eliminar(actor, gerente);
        Console.WriteLine("OK limpieza limitada a los registros propios de esta ejecución: " + Prefijo);
    }

    private static async Task<int> Main(string[] args)
    {
        if (args.FirstOrDefault() == "--final") return await ProbarWebFinal(args.Skip(1).FirstOrDefault());
        if (args.FirstOrDefault() == "--procesos") return await ProbarProcesosWeb(args.Skip(1).FirstOrDefault());
        UsuarioSesion? gerente = null;
        try
        {
            if (args.Length > 0) servidor = args[0];
            string passwordGerente = Environment.GetEnvironmentVariable("PRUEBA_WEB_GERENTE_PASSWORD") ?? throw new InvalidOperationException("Falta PRUEBA_WEB_GERENTE_PASSWORD.");
            string passwordCajero = Environment.GetEnvironmentVariable("PRUEBA_WEB_CAJERO_PASSWORD") ?? throw new InvalidOperationException("Falta PRUEBA_WEB_CAJERO_PASSWORD.");
            gerente = Usuarios.IniciarSesion("gerente", passwordGerente) ?? throw new InvalidOperationException("No se pudo autenticar Gerente.");
            int rolDigitador = new RolNegocio().ListarRoles().Single(r => r.NombreRol == "Digitador").ID_Rol;
            string passwordTemporal = Guid.NewGuid().ToString("N") + "A1!";
            Usuarios.Guardar(new Usuario { Nombres = "Prueba web", Apellidos = "Temporal", NombreUsuario = Prefijo, ID_Rol = rolDigitador }, passwordTemporal, gerente, true);
            actor = Usuarios.Listar(gerente).Single(u => u.NombreUsuario == Prefijo).ID_Usuario;
            var digitador = Usuarios.IniciarSesion(Prefijo, passwordTemporal)!;
            using var anonimoHttp = ClienteHttp();
            using var gerenteHttp = ClienteHttp();
            using var cajeroHttp = ClienteHttp();
            using var digitadorHttp = ClienteHttp();
            await Entrar(gerenteHttp, "gerente", passwordGerente);
            await Entrar(cajeroHttp, "cajero", passwordCajero);
            await Entrar(digitadorHttp, Prefijo, passwordTemporal);
            Exigir((await Pagina(anonimoHttp, "/health")).Contains("Healthy"), "No responde /health.");
            Console.WriteLine("PRUEBAS WEB | " + DateTime.Now.ToString("s"));
            await Seguridad(anonimoHttp, gerenteHttp, cajeroHttp, digitadorHttp);
            await Crud(digitadorHttp, gerente, digitador);
            Console.WriteLine("RESULTADO: mantenimiento Web verificado por HTTP contra SQL Server, sin cambios de esquema ni procedimientos.");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
        finally { if (gerente != null) Limpiar(gerente); }
    }
}
