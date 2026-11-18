using System.Data;
using System.Drawing;
using Microsoft.Data.SqlClient;
using ProyectoBD.Datos;
using ProyectoBD.Entidades;
using ProyectoBD.Negocio;
using ProyectoBD.Presentacion;

internal static partial class Program
{
    private static bool integrarUI;
    [STAThread]
    static int Main(string[] args)
    {
        try
        {
            integrarUI = args.Contains("--integracion-ui");
            if (args.Contains("--fase-final")) { ProbarFaseFinal(); return 0; }
            if (args.Contains("--fase-final-ui")) { MostrarFaseFinal(); return 0; }
            if (args.Contains("--fase-final-pantallas")) { ValidarPantallasFinal(); return 0; }
            if (args.Contains("--verificar-login")) { VerificarLogin(); return 0; }
            if (args.Contains("--crear-gerente"))
            {
                CrearGerente();
                return 0;
            }
            if (args.Contains("--diseno")) { VerificarDiseno(); return 0; }
            if (args.Contains("--ui"))
            {
                ProbarFormularios();
                return 0;
            }
            ProbarFase1();
            return 0;
        }
        catch (Exception e) { Console.Error.WriteLine(e); return 1; }
    }


    private static void VerificarLogin()
    {
        ApplicationConfiguration.Initialize();
        UsuarioNegocio negocio = new UsuarioNegocio();
        foreach (var cuenta in new[] { (Nombre: "cajero", Password: "cajero123", Rol: "Cajero"), (Nombre: "gerente", Password: "gerente 123", Rol: "Gerente") })
        {
            UsuarioSesion? sesion = negocio.IniciarSesion(cuenta.Nombre, cuenta.Password);
            Afirmar(sesion != null && sesion.NombreRol == cuenta.Rol, "No autentica " + cuenta.Nombre);
            using (ProyectoBD.Presentacion.Menu menu = new ProyectoBD.Presentacion.Menu(sesion!))
            {
                menu.StartPosition = FormStartPosition.Manual; menu.Location = new Point(-2500, -2500);
                menu.Show(); Application.DoEvents();
                Afirmar(menu.Visible, "No abre el menú de " + cuenta.Nombre);
                menu.Close();
            }
            Afirmar(negocio.IniciarSesion(cuenta.Nombre, "contraseña incorrecta") == null, "Acepta contraseña incorrecta.");
            Console.WriteLine("OK: autenticación y apertura del menú WinForms de " + cuenta.Nombre);
        }
        using (SqlConnection conexion = new ConexionBD().CrearConexion())
        {
            conexion.Open();
            Console.WriteLine("Servidor resuelto: " + conexion.DataSource);
            Console.WriteLine("Base de datos: " + conexion.Database);
            Afirmar(conexion.Database == "Proyecto_BD", "Se conectó a otra base.");
        }
        Console.WriteLine("SQL Client: " + typeof(SqlConnection).Assembly.Location);
        Console.WriteLine("Runtime: " + System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription);
    }

    private static void CrearGerente()
    {
        var rol = new RolNegocio().ListarRoles().Single(x => x.NombreRol == "Gerente");
        if (new UsuarioDatos().Listar().Any(x => x.ID_Rol == rol.ID_Rol))
            throw new InvalidOperationException("Ya existe un Gerente. Ingrese con ese usuario para administrar las cuentas.");
        Console.Write("Nombres: ");
        string nombres = Console.ReadLine() ?? "";
        Console.Write("Apellidos: ");
        string apellidos = Console.ReadLine() ?? "";
        Console.Write("Nombre de usuario: ");
        string nombre = Console.ReadLine() ?? "";
        Console.Write("Contraseña (mínimo 6 caracteres; no se muestra): ");
        string password = LeerPassword();
        Console.Write("Repita la contraseña: ");
        string repetida = LeerPassword();
        if (password != repetida || string.IsNullOrWhiteSpace(password) || password.Length < 6 || System.Text.Encoding.UTF8.GetByteCount(password) > 72)
            throw new ArgumentException("Las contraseñas deben coincidir y tener al menos 6 caracteres y hasta 72 bytes UTF-8.");
        if (new[] { nombres, apellidos, nombre }.Any(x => string.IsNullOrWhiteSpace(x) || x.Trim().Length > 45))
            throw new ArgumentException("Complete nombres, apellidos y usuario (hasta 45 caracteres).");
        if (!new UsuarioNegocio().CrearUsuarioInicial(nombres, apellidos, nombre, password, rol.ID_Rol))
            throw new InvalidOperationException("No se creó la cuenta.");
        Console.WriteLine("Gerente creado. Inicie sesión en Windows Forms con las credenciales ingresadas.");
    }
    private static string LeerPassword()
    {
        var texto = new System.Text.StringBuilder();
        ConsoleKeyInfo tecla;
        while ((tecla = Console.ReadKey(true)).Key != ConsoleKey.Enter)
        {
            if (tecla.Key == ConsoleKey.Backspace)
            {
                if (texto.Length > 0)
                    texto.Length--;
            }
            else if (!char.IsControl(tecla.KeyChar))
                texto.Append(tecla.KeyChar);
        }
        Console.WriteLine();
        return texto.ToString();
    }

    private static void Afirmar(bool condicion, string mensaje)
    {
        if (!condicion)
            throw new Exception(mensaje);
    }
    private static void ProbarFase1()
    {
        string sufijo = Guid.NewGuid().ToString("N")[..12];
        int actor = 0, usuario = 0, rol = 0, marca = 0, proveedor = 0, cliente = 0, producto = 0, variante = 0, categoria = 0, presentacion = 0, color = 0;
        try
        {
            var un = new UsuarioNegocio();
            var roles = new RolNegocio();
            int gerente = roles.ListarRoles().Single(x => x.NombreRol == "Gerente").ID_Rol;
            Afirmar(un.CrearUsuarioInicial("Prueba", "Gerente", "t" + sufijo, "Prueba123!", gerente), "No creó actor de prueba.");
            var sesion = un.IniciarSesion("t" + sufijo, "Prueba123!") ?? throw new Exception("Falló login BCrypt.");
            actor = sesion.ID_Usuario;
            Afirmar(un.IniciarSesion("t" + sufijo, "incorrecta") == null, "Aceptó contraseña incorrecta.");
            roles.Guardar(new Rol { NombreRol = "Rol" + sufijo });
            rol = roles.ListarRoles().Single(x => x.NombreRol == "Rol" + sufijo).ID_Rol;
            roles.Actualizar(new Rol { ID_Rol = rol, NombreRol = "Editado" + sufijo });
            un.Guardar(new Usuario { Nombres = "Prueba", Apellidos = "Usuario", NombreUsuario = "u" + sufijo, ID_Rol = rol }, "Inicial123!", sesion, true);
            usuario = un.Listar(sesion).Single(x => x.NombreUsuario == "u" + sufijo).ID_Usuario;
            Afirmar(un.IniciarSesion("u" + sufijo, "Inicial123!") != null, "Falló contraseña de usuario nuevo.");
            bool bloqueado = false;
            try
            {
                roles.Eliminar(rol);
            }
            catch (InvalidOperationException) { bloqueado = true; }
            Afirmar(bloqueado, "Permitió borrar rol asignado.");
            var u = un.Listar(sesion).Single(x => x.ID_Usuario == usuario);
            u.Nombres = "Editado";
            un.Guardar(u, "", sesion, false);
            un.CambiarPassword(usuario, "Nueva123!", sesion);
            Afirmar(un.IniciarSesion("u" + sufijo, "Inicial123!") == null && un.IniciarSesion("u" + sufijo, "Nueva123!") != null, "Falló cambio BCrypt.");
            Afirmar(un.Listar(sesion).All(x => x.PasswordHash == ""), "El listado expone hashes.");
            bool corta = false;
            try
            {
                un.CambiarPassword(usuario, "123", sesion);
            }
            catch (ArgumentException) { corta = true; }
            Afirmar(corta, "Aceptó contraseña corta.");
            un.Eliminar(usuario, sesion);
            usuario = 0;
            roles.Eliminar(rol);
            rol = 0;
            Console.WriteLine("OK: roles CRUD, bloqueo de rol asignado, usuarios CRUD, BCrypt, cambio de contraseña, validación y listado sin hashes.");
            var mn = new MarcaNegocio();
            mn.Guardar(new Marca { NombreMarca = "m" + sufijo });
            marca = mn.Listar().Single(x => x.NombreMarca == "m" + sufijo).ID_Marca;
            mn.Actualizar(new Marca { ID_Marca = marca, NombreMarca = "edit" + sufijo });
            var pn = new ProveedorNegocio();
            pn.Guardar(new Proveedor { Nombre_Proveedor = "p" + sufijo });
            proveedor = pn.Listar().Single(x => x.Nombre_Proveedor == "p" + sufijo).ID_Proveedor;
            pn.Actualizar(new Proveedor { ID_Proveedor = proveedor, Nombre_Proveedor = "edit" + sufijo });
            var cn = new ClienteNegocio();
            cn.Guardar(new Cliente { TipoCliente = "Particular", NombreRazonSocial = "c" + sufijo });
            cliente = cn.Listar().Single(x => x.NombreRazonSocial == "c" + sufijo).ID_Cliente;
            cn.Actualizar(new Cliente { ID_Cliente = cliente, TipoCliente = "Empresa", NombreRazonSocial = "edit" + sufijo });
            var cat = new CategoriaNegocio();
            cat.Guardar(new Categoria { NombreCategoria = "cat" + sufijo, Descripcion = "Prueba" });
            categoria = cat.Listar().Single(x => x.NombreCategoria == "cat" + sufijo).ID_Categoria;
            var pr = new PresentacionNegocio();
            pr.Guardar(new ProyectoBD.Entidades.Presentacion { NombrePresentacion = "pr" + sufijo, UnidadMedida = "Unidad" });
            presentacion = pr.Listar().Single(x => x.NombrePresentacion == "pr" + sufijo).ID_Presentacion;
            var co = new ColorProductoNegocio();
            co.Guardar(new ColorProducto { NombreColor = "co" + sufijo });
            color = co.Listar().Single(x => x.NombreColor == "co" + sufijo).ID_Color;
            var prod = new ProductoNegocio();
            prod.Guardar(new Producto { NombreProducto = "prod" + sufijo, Descripcion = "Prueba", ID_CategoriaP = categoria, ID_Marca = marca }, true);
            var item = prod.Listar().Single(x => x.NombreProducto == "prod" + sufijo);
            producto = item.ID_Producto;
            item.Descripcion = "Editada";
            prod.Guardar(item, false);
            prod.GuardarVariante(new ProductoVariante { ID_Producto = producto, ID_Presentacion = presentacion, ID_Color = null, PrecioVenta = 10 }, true);
            var v = prod.ListarVariantes(producto).Single();
            variante = v.ID_ProductoV;
            Afirmar(v.ID_Color == null, "No conserva color NULL.");
            v.ID_Color = color;
            v.PrecioVenta = 12.34m;
            prod.GuardarVariante(v, false);
            Afirmar(prod.ListarVariantes(producto).Single().PrecioVenta == 12.34m, "No editó variante.");
            ProbarProcesos(un, sesion, sufijo, cliente, proveedor, variante);
            prod.EliminarVariante(variante);
            variante = 0;
            prod.Eliminar(producto);
            producto = 0;
            mn.Eliminar(marca);
            marca = 0;
            pn.Eliminar(proveedor);
            proveedor = 0;
            cn.Eliminar(cliente);
            cliente = 0;
            cat.Eliminar(categoria);
            categoria = 0;
            pr.Eliminar(presentacion);
            presentacion = 0;
            co.Eliminar(color);
            color = 0;
            using var c = new ConexionBD().CrearConexion();
            c.Open();
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.tbl_Auditoria WHERE ID_Usuario=@id AND (DatosAnteriores LIKE '%PasswordHash%' OR DatosNuevos LIKE '%PasswordHash%'); SELECT COUNT(*) FROM dbo.tbl_Auditoria WHERE ID_Usuario=@id;", c);
            cmd.Parameters.Add("@id", SqlDbType.Int).Value = actor;
            using var r = cmd.ExecuteReader();
            r.Read();
            Afirmar(r.GetInt32(0) == 0, "Auditoría contiene hash.");
            r.NextResult();
            r.Read();
            Afirmar(r.GetInt32(0) >= 4, "Falta auditoría de usuarios.");
            Console.WriteLine("OK: marcas, proveedores, clientes, productos y variantes CRUD; color NULL y auditoría sin hashes.");
        }
        finally
        {
            // Limpieza limitada a las identidades creadas por esta ejecución.
            using var c = new ConexionBD().CrearConexion();
            c.Open();
            using var cmd = new SqlCommand(@"SET XACT_ABORT ON; BEGIN TRAN;
DELETE dbo.tbl_ProductoVariante WHERE ID_ProductoV=@variante; DELETE dbo.tbl_Producto WHERE ID_Producto=@producto;
DELETE dbo.tbl_Marca WHERE ID_Marca=@marca; DELETE dbo.tbl_Proveedor WHERE ID_Proveedor=@proveedor; DELETE dbo.tbl_Clientes WHERE ID_Cliente=@cliente;
DELETE dbo.tbl_CategoriaProducto WHERE ID_Categoria=@categoria; DELETE dbo.tbl_Presentacion WHERE ID_Presentacion=@presentacion; DELETE dbo.tbl_Color WHERE ID_Color=@color;
DELETE dbo.tbl_Auditoria WHERE ID_Usuario=@actor; DELETE dbo.tbl_Usuario WHERE ID_Usuario=@usuario OR ID_Usuario=@actor; DELETE dbo.tbl_Rol WHERE ID_Rol=@rol; COMMIT;", c);
            foreach (var par in new[] { ("@actor", actor), ("@usuario", usuario), ("@rol", rol), ("@marca", marca), ("@proveedor", proveedor), ("@cliente", cliente), ("@producto", producto), ("@variante", variante), ("@categoria", categoria), ("@presentacion", presentacion), ("@color", color) })
                cmd.Parameters.Add(par.Item1, SqlDbType.Int).Value = par.Item2;
            cmd.ExecuteNonQuery();
            Console.WriteLine("OK: limpieza de datos temporales de verificación.");
        }
    }

    private static void ProbarProcesos(UsuarioNegocio usuarios, UsuarioSesion gerente, string sufijo, int cliente, int proveedor, int variante)
    {
        int digitadorId = 0, cajeroId = 0, ubicacion = 0, metodo = 0;
        try
        {
            var roles = new RolNegocio().ListarRoles();
            usuarios.Guardar(new Usuario { Nombres = "Proceso", Apellidos = "Digitador", NombreUsuario = "d" + sufijo, ID_Rol = roles.Single(x => x.NombreRol == "Digitador").ID_Rol }, "Prueba123!", gerente, true);
            var digitador = usuarios.IniciarSesion("d" + sufijo, "Prueba123!")!;
            digitadorId = digitador.ID_Usuario;
            usuarios.Guardar(new Usuario { Nombres = "Proceso", Apellidos = "Cajero", NombreUsuario = "j" + sufijo, ID_Rol = roles.Single(x => x.NombreRol == "Cajero").ID_Rol }, "Prueba123!", gerente, true);
            var cajero = usuarios.IniciarSesion("j" + sufijo, "Prueba123!")!;
            cajeroId = cajero.ID_Usuario;
            var ub = new UbicacionNegocio();
            ub.Guardar(new Ubicacion { NombreUbicacion = "u" + sufijo, Descripcion = "Prueba" });
            ubicacion = ub.Listar().Single(x => x.NombreUbicacion == "u" + sufijo).ID_UbicacionA;
            var me = new MetodoPagoNegocio();
            me.Guardar(new MetodoPago { NombreMetodo = "m" + sufijo });
            metodo = me.Listar().Single(x => x.NombreMetodo == "m" + sufijo).ID_MetodoP;
            var n = new ProcesoNegocio(digitador);
            var caja = new ProcesoNegocio(cajero);
            int orden = n.GuardarOrden(new ProyectoBD.Entidades.OrdenCompra { ID_Proveedor = proveedor, Detalle = new() { new() { ID_ProductoV = variante, Cantidad = 90 } } });
            int doc = Convert.ToInt32(n.PendienteOrden(orden).Rows[0]["ID_DetalleOC"]);
            n.Ingresar(new ProyectoBD.Entidades.IngresoBodega { ID_OrdenC = orden, ID_UbicacionA = ubicacion, FechaIngreso = DateTime.Now.AddDays(-2), Detalle = new() { new() { ID_DetalleOC = doc, CantidadRecibida = 50, CostoUnitario = 5 } } });
            n.Ingresar(new ProyectoBD.Entidades.IngresoBodega { ID_OrdenC = orden, ID_UbicacionA = ubicacion, FechaIngreso = DateTime.Now.AddDays(-1), Detalle = new() { new() { ID_DetalleOC = doc, CantidadRecibida = 40, CostoUnitario = 6 } } });
            Afirmar((string)n.Ordenes().Select($"ID_OrdenC={orden}").Single()["Estado"] == "Completa", "No completó compra desde C#.");
            int factura = n.Facturar(new ProyectoBD.Entidades.Factura { ID_Cliente = cliente, Detalle = new() { new() { ID_ProductoV = variante, Cantidad = 60, PrecioUnitario = 10 } } });
            // Dos conexiones concurrentes intentan vender 20 de un saldo de 30.
            Func<bool> vender = () => { try { new ProcesoNegocio(cajero).Facturar(new ProyectoBD.Entidades.Factura { ID_Cliente = cliente, Detalle = new() { new() { ID_ProductoV = variante, Cantidad = 20, PrecioUnitario = 10 } } }); return true; } catch (InvalidOperationException ex) when (ex.Message.Contains("Inventario insuficiente")) { return false; } };
            var tareas = new[] { Task.Run(vender), Task.Run(vender) };
            Task.WaitAll(tareas);
            Afirmar(tareas.Count(t => t.Result) == 1, "Las ventas concurrentes no protegieron existencias.");
            int cot = n.GuardarCotizacion(new ProyectoBD.Entidades.Cotizacion { ID_Cliente = cliente, Detalle = new() { new() { ID_ProductoV = variante, Cantidad = 10, PrecioUnitario = 10 } } });
            n.EstadoCotizacion(cot, "Confirmada");
            caja.Facturar(new ProyectoBD.Entidades.Factura { ID_Cliente = cliente, ID_Cotizacion = cot });
            caja.Pagar(factura, new() { new() { ID_MetodoP = metodo, Monto = 300 }, new() { ID_MetodoP = metodo, Monto = 200 }, new() { ID_MetodoP = metodo, Monto = 100 } });
            Afirmar((string)caja.Facturas().Select($"ID_Factura={factura}").Single()["Estado"] == "Pagada", "Pago mixto desde C# no completó total.");
            Afirmar(n.Inventario().AsEnumerable().Where(r => r.Field<int>("ID_Lote") > 0 && r.Field<string>("NombreProducto") == "prod" + sufijo).Sum(r => r.Field<int>("CantidadDisponible")) == 0, "Inventario final incorrecto.");
            bool bloqueado = false;
            try
            {
                caja.GuardarOrden(new ProyectoBD.Entidades.OrdenCompra { ID_Proveedor = proveedor, Detalle = new() { new() { ID_ProductoV = variante, Cantidad = 1 } } });
            }
            catch (InvalidOperationException) { bloqueado = true; }
            Afirmar(bloqueado, "Negocio no aplicó permiso de Cajero.");
            if (integrarUI)
            {
                n.GuardarOrden(new ProyectoBD.Entidades.OrdenCompra { ID_Proveedor = proveedor, Detalle = new() { new() { ID_ProductoV = variante, Cantidad = 5 } } });
                int cotConfirmada = n.GuardarCotizacion(new ProyectoBD.Entidades.Cotizacion { ID_Cliente = cliente, Detalle = new() { new() { ID_ProductoV = variante, Cantidad = 1, PrecioUnitario = 10 } } });
                n.EstadoCotizacion(cotConfirmada, "Confirmada");
                ApplicationConfiguration.Initialize();
                System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener("Documentacion/ErroresUI.txt"));
                System.Diagnostics.Trace.AutoFlush = true;
                using var limite = new System.Threading.Timer(_ => Environment.Exit(2), null, 30000, Timeout.Infinite);
                foreach (var form in new Form[] { new Productos(), new ProyectoBD.Presentacion.Usuarios(gerente), new ProyectoBD.Presentacion.Cotizacion(digitador), new ProyectoBD.Presentacion.OrdenCompra(digitador), new ProyectoBD.Presentacion.IngresoBodega(digitador), new ProyectoBD.Presentacion.Factura(cajero), new ProyectoBD.Presentacion.Pagos(cajero, factura) })
                {
                    using (form)
                    {
                        form.StartPosition = FormStartPosition.Manual;
                        form.Location = new Point(-2500, -2500);
                        form.Show();
                        Application.DoEvents();
                        if (form is ProyectoBD.Presentacion.Cotizacion || form is ProyectoBD.Presentacion.OrdenCompra || form is ProyectoBD.Presentacion.Factura)
                        {
                            Type tipoFormulario = form.GetType();
                            var flags = System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic;
                            ComboBox variantes = (ComboBox)tipoFormulario.GetField("comboBox1", flags)!.GetValue(form)!;
                            NumericUpDown cantidad = (NumericUpDown)tipoFormulario.GetField("numericUpDown1", flags)!.GetValue(form)!;
                            NumericUpDown precio = (NumericUpDown)tipoFormulario.GetField("numericUpDown2", flags)!.GetValue(form)!;
                            Button agregar = (Button)tipoFormulario.GetField("button1", flags)!.GetValue(form)!;
                            Button quitar = (Button)tipoFormulario.GetField("button2", flags)!.GetValue(form)!;
                            DataGridView tablaDetalle = (DataGridView)tipoFormulario.GetField("dataGridView1", flags)!.GetValue(form)!;
                            variantes.SelectedValue = variante; cantidad.Value = 2; precio.Value = 10;
                            agregar.PerformClick(); Application.DoEvents();
                            Afirmar(tablaDetalle.Rows.Count == 1, "Agregar detalle no funcionó en " + form.Name);
                            Afirmar(Convert.ToInt32(tablaDetalle.Rows[0].Cells["Cantidad"].Value) == 2, "Cantidad incorrecta en " + form.Name);
                            quitar.PerformClick(); Application.DoEvents();
                            Afirmar(tablaDetalle.Rows.Count == 0, "Quitar detalle no funcionó en " + form.Name);
                            agregar.PerformClick(); Application.DoEvents();
                        }
                        form.Refresh(); Application.DoEvents();
                        Capturar(form, form.GetType().Name + "-ConDatos");
                        if (form is ProyectoBD.Presentacion.Factura)
                        {
                            var combo = (ComboBox)typeof(ProyectoBD.Presentacion.Factura).GetField("comboBox2", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(form)!;
                            combo.SelectedValue = cotConfirmada;
                            Application.DoEvents();
                            Button agregar = (Button)typeof(ProyectoBD.Presentacion.Factura).GetField("button1", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.GetValue(form)!;
                            Afirmar(!agregar.Enabled, "La cotización confirmada permite alterar las líneas.");
                            Capturar(form, "Factura-CotizacionConfirmada");
                        }
                        foreach (var tab in Descendientes(form).OfType<TabControl>())
                        foreach (TabPage page in tab.TabPages)
                        {
                            tab.SelectedTab = page;
                            Application.DoEvents();
                            Capturar(form, form.GetType().Name + "-ConDatos-" + tab.SelectedIndex);
                        }
                        Button? volver = Descendientes(form).OfType<Button>().FirstOrDefault(b => b.Text == "Volver");
                        Afirmar(volver != null && volver.Visible, "Falta Volver en " + form.Name);
                        volver!.PerformClick(); Application.DoEvents(); Afirmar(!form.Visible, "Volver no cierra " + form.Name);
                        form.Close();
                    }
                }
                var tipo = typeof(ProyectoBD.Presentacion.Factura).Assembly.GetType("ProyectoBD.Presentacion.ImpresionFactura")!;
                using var documento = (System.Drawing.Printing.PrintDocument)tipo.GetMethod("CrearDocumento", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)!.Invoke(null, new object[] { caja, factura })!;
                var controlador = new System.Drawing.Printing.PreviewPrintController();
                documento.PrintController = controlador;
                documento.Print();
                var paginas = controlador.GetPreviewPageInfo();
                Afirmar(paginas.Length > 0, "La factura no generó páginas.");
                Directory.CreateDirectory("Documentacion/CapturasFormularios");
                for (int i = 0; i < paginas.Length; i++)
                {
                    using var raster = new Bitmap(1100, 1555);
                    using (var g = Graphics.FromImage(raster))
                    {
                        g.Clear(System.Drawing.Color.White);
                        g.DrawImage(paginas[i].Image, new Rectangle(0, 0, raster.Width, raster.Height));
                    }
                    raster.Save($"Documentacion/CapturasFormularios/FacturaImpresion-{i + 1}.png", System.Drawing.Imaging.ImageFormat.Png);
                    paginas[i].Image.Dispose();
                }
                Console.WriteLine("OK: formularios cargados con datos reales y factura renderizada con PreviewPrintController, sin imprimir físicamente.");
            }
            Console.WriteLine("OK: flujo C# compra → dos ingresos → PEPS → cotización/factura → pagos JSON; dos ventas concurrentes no sobrevendieron; permisos de Negocio.");
        }
        finally
        {
            using var c = new ConexionBD().CrearConexion();
            c.Open();
            using var cmd = new SqlCommand(@"SET XACT_ABORT ON; BEGIN TRAN;
DELETE dbo.tbl_PagoFactura WHERE ID_Factura IN(SELECT ID_Factura FROM dbo.tbl_Factura WHERE ID_Cliente=@cliente);
DELETE dbo.tbl_DetalleFacturaLote WHERE ID_DetalleF IN(SELECT d.ID_DetalleF FROM dbo.tbl_DetalleFactura d JOIN dbo.tbl_Factura f ON f.ID_Factura=d.ID_Factura WHERE f.ID_Cliente=@cliente);
DELETE dbo.tbl_DetalleFactura WHERE ID_Factura IN(SELECT ID_Factura FROM dbo.tbl_Factura WHERE ID_Cliente=@cliente);
DELETE dbo.tbl_Factura WHERE ID_Cliente=@cliente; DELETE dbo.tbl_DetalleCotizacion WHERE ID_Cotizacion IN(SELECT ID_Cotizacion FROM dbo.tbl_Cotizacion WHERE ID_Cliente=@cliente); DELETE dbo.tbl_Cotizacion WHERE ID_Cliente=@cliente;
DELETE dbo.tbl_MovimientoInventario WHERE ID_ProductoV=@variante; DELETE dbo.tbl_LoteInventario WHERE ID_ProductoV=@variante;
DELETE dbo.tbl_DetalleIngresoBodega WHERE ID_IngresoB IN(SELECT i.ID_IngresoB FROM dbo.tbl_IngresoBodega i JOIN dbo.tbl_OrdenCompra o ON o.ID_OrdenC=i.ID_OrdenC WHERE o.ID_Proveedor=@proveedor);
DELETE dbo.tbl_IngresoBodega WHERE ID_OrdenC IN(SELECT ID_OrdenC FROM dbo.tbl_OrdenCompra WHERE ID_Proveedor=@proveedor);
DELETE dbo.tbl_DetalleOrdenCompra WHERE ID_OrdenC IN(SELECT ID_OrdenC FROM dbo.tbl_OrdenCompra WHERE ID_Proveedor=@proveedor); DELETE dbo.tbl_OrdenCompra WHERE ID_Proveedor=@proveedor;
DELETE dbo.tbl_Auditoria WHERE ID_Usuario=@digitador OR ID_Usuario=@cajero; DELETE dbo.tbl_Usuario WHERE ID_Usuario=@digitador OR ID_Usuario=@cajero; DELETE dbo.tbl_UbicacionAlmacen WHERE ID_UbicacionA=@ubicacion; DELETE dbo.tbl_MetodoPago WHERE ID_MetodoP=@metodo; COMMIT;", c);
            foreach (var par in new[] { ("@cliente", cliente), ("@proveedor", proveedor), ("@variante", variante), ("@digitador", digitadorId), ("@cajero", cajeroId), ("@ubicacion", ubicacion), ("@metodo", metodo) })
                cmd.Parameters.Add(par.Item1, SqlDbType.Int).Value = par.Item2;
            cmd.ExecuteNonQuery();
        }
    }

    private static IEnumerable<Control> Descendientes(Control control)
    {
        foreach (Control hijo in control.Controls)
        {
            yield return hijo;
            foreach (var nieto in Descendientes(hijo))
                yield return nieto;
        }
    }
    private static void Capturar(Form f, string nombre)
    {
        Directory.CreateDirectory("Documentacion/CapturasFormularios");
        using var bitmap = new Bitmap(f.Width, f.Height);
        f.DrawToBitmap(bitmap, new Rectangle(0, 0, f.Width, f.Height));
        bitmap.Save(Path.Combine("Documentacion/CapturasFormularios", nombre + ".png"));
    }
    
        
        private sealed class SitioDiseno : System.ComponentModel.ISite
        {
            public SitioDiseno(System.ComponentModel.IComponent componente) { Component = componente; }
            public System.ComponentModel.IComponent Component { get; }
            public System.ComponentModel.IContainer? Container { get { return null; } }
            public bool DesignMode { get { return true; } }
            public string? Name { get; set; }
            public object? GetService(Type serviceType) { return null; }
        }

        private static void VerificarDiseno()
        {
            ApplicationConfiguration.Initialize();
            Form[] formularios = new Form[]
            {
                new Roles(), new ProyectoBD.Presentacion.Usuarios(), new Marcas(), new Proveedores(),
                new Clientes(), new Productos(), new Categorias(), new Presentaciones(), new Colores(),
                new ProyectoBD.Presentacion.Ubicaciones(), new ProyectoBD.Presentacion.MetodosPago(),
                new ProyectoBD.Presentacion.Cotizacion(), new ProyectoBD.Presentacion.OrdenCompra(),
                new ProyectoBD.Presentacion.IngresoBodega(), new ProyectoBD.Presentacion.Factura(),
                new ProyectoBD.Presentacion.Pagos(), new Reportes(), new ProyectoBD.Presentacion.Menu()
            };
            foreach (Form formulario in formularios)
            {
                using (formulario)
                {
                    Afirmar(formulario.FormBorderStyle == FormBorderStyle.None, formulario.Name + " tiene barra superior.");
                    Afirmar(formulario.BackColor == System.Drawing.Color.DarkCyan, formulario.Name + " no usa el color del menú.");
                    Afirmar(!Descendientes(formulario).OfType<TabControl>().Any(), formulario.Name + " conserva pestañas.");
                    Afirmar(formulario.Controls.Count > 0, formulario.Name + " no tiene controles al construirse.");
                    if (!(formulario is ProyectoBD.Presentacion.Menu))
                    {
                        Button? volver = Descendientes(formulario).OfType<Button>().FirstOrDefault(b => b.Text == "Volver");
                        Afirmar(volver != null, formulario.Name + " no tiene botón Volver.");
                    }
                    formulario.Site = new SitioDiseno(formulario);
                    formulario.StartPosition = FormStartPosition.Manual;
                    formulario.Location = new Point(-2500, -2500);
                    formulario.Show(); Application.DoEvents();
                    Capturar(formulario, formulario.GetType().Name + "-Diseno");
                    formulario.Close();
                    Console.WriteLine("OK diseñador: " + formulario.GetType().Name + " — controles presentes antes de Load, color DarkCyan, sin barra ni pestañas.");
                }
            }
        }

        private static void ProbarFormularios()
    {
        ApplicationConfiguration.Initialize();
        System.Diagnostics.Trace.Listeners.Add(new System.Diagnostics.TextWriterTraceListener("Documentacion/ErroresUI.txt"));
        System.Diagnostics.Trace.AutoFlush = true;
        string salida = Path.GetFullPath("Documentacion/CapturasFormularios");
        Directory.CreateDirectory(salida);
        var digitador = new UsuarioSesion { ID_Usuario = 1, NombreRol = "Digitador", NombreCompleto = "Prueba" };
        var gerente = new UsuarioSesion { ID_Usuario = 1, NombreRol = "Gerente", NombreCompleto = "Prueba" };
        var cajero = new UsuarioSesion { ID_Usuario = 1, NombreRol = "Cajero", NombreCompleto = "Prueba" };
        var fabricas = new Func<Form>[] { () => new Roles(), () => new ProyectoBD.Presentacion.Usuarios(gerente), () => new Marcas(), () => new Proveedores(), () => new Clientes(), () => new Productos(), () => new Categorias(), () => new Presentaciones(), () => new Colores(), () => new ProyectoBD.Presentacion.Ubicaciones(), () => new ProyectoBD.Presentacion.MetodosPago(), () => new ProyectoBD.Presentacion.Cotizacion(digitador), () => new ProyectoBD.Presentacion.OrdenCompra(digitador), () => new ProyectoBD.Presentacion.IngresoBodega(digitador), () => new ProyectoBD.Presentacion.Factura(digitador), () => new ProyectoBD.Presentacion.Menu(digitador), () => new ProyectoBD.Presentacion.Menu(gerente), () => new ProyectoBD.Presentacion.Menu(cajero) };
        using var limite = new System.Threading.Timer(_ => { Console.Error.WriteLine("ERROR: formulario bloqueado o diálogo de error inesperado."); Environment.Exit(2); }, null, 30000, Timeout.Infinite);
        foreach (var fabrica in fabricas)
        {
            using var f = fabrica();
            f.StartPosition = FormStartPosition.Manual;
            f.Location = new Point(-2500, -2500);
            f.Show();
            Application.DoEvents();
            using var bitmap = new Bitmap(f.Width, f.Height);
            f.DrawToBitmap(bitmap, new Rectangle(0, 0, f.Width, f.Height));
            string nombre = f.GetType().Name + (f is ProyectoBD.Presentacion.Menu ? $"-{(f.Controls.OfType<Label>().FirstOrDefault(x => x.Text.StartsWith("Rol:"))?.Text ?? "").Replace(":", "").Replace(" ", "")}" : "");
            bitmap.Save(Path.Combine(salida, nombre + ".png"));
            Button? volver = Descendientes(f).OfType<Button>().FirstOrDefault(b => b.Text == "Volver");
            if (volver != null)
            {
                Console.WriteLine("Volver " + nombre + ": " + volver.Bounds + ", visible=" + volver.Visible + ", cliente=" + f.ClientSize);
                Afirmar(volver.Visible, "El botón Volver está oculto en " + nombre);
                Afirmar(f.ClientRectangle.Contains(volver.Bounds) || volver.Parent != f, "Volver quedó fuera de " + nombre);
                volver.PerformClick();
                Application.DoEvents();
                Afirmar(!f.Visible, "Volver no cerró " + nombre);
            }
            Console.WriteLine("OK formulario: " + nombre);
            f.Close();
        }
    }
}



