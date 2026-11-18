using ProyectoBD.Entidades;
namespace ProyectoBD.Negocio;

public partial class UsuarioNegocio
{
    public List<Usuario> Listar(UsuarioSesion sesion)
    {
        PermisosNegocio.Exigir(sesion, "Gerente");
        return usuarioDatos.Listar();
    }
    public bool Guardar(Usuario usuario, string password, UsuarioSesion sesion, bool nuevo)
    {
        PermisosNegocio.Exigir(sesion, "Gerente");
        if (new[] { usuario.Nombres, usuario.Apellidos, usuario.NombreUsuario }.Any(x => string.IsNullOrWhiteSpace(x) || x.Trim().Length > 45) || usuario.ID_Rol <= 0)
            throw new ArgumentException("Complete nombres, apellidos, usuario (máximo 45 caracteres) y rol.");
        if (!nuevo && usuario.ID_Usuario <= 0)
            throw new ArgumentException("Seleccione un usuario.");
        if (usuario.ID_Usuario == sesion.ID_Usuario && usuario.ID_Rol != sesion.ID_Rol)
            throw new ArgumentException("No puede cambiar su propio rol durante la sesión.");
        usuario.Nombres = usuario.Nombres.Trim();
        usuario.Apellidos = usuario.Apellidos.Trim();
        usuario.NombreUsuario = usuario.NombreUsuario.Trim();
        if (nuevo)
        {
            ValidarPassword(password);
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }
        return usuarioDatos.Guardar(usuario, sesion.ID_Usuario, nuevo);
    }
    private static void ValidarPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 6 || System.Text.Encoding.UTF8.GetByteCount(password) > 72)
            throw new ArgumentException("La contraseña debe tener al menos 6 caracteres y no superar 72 bytes UTF-8.");
    }
    public bool CambiarPassword(int idUsuario, string nuevaPassword, UsuarioSesion sesion)
    {
        PermisosNegocio.Exigir(sesion, "Gerente");
        if (idUsuario <= 0)
            throw new ArgumentException("Seleccione un usuario.");
        ValidarPassword(nuevaPassword);
        return usuarioDatos.ActualizarPassword(idUsuario, BCrypt.Net.BCrypt.HashPassword(nuevaPassword), sesion.ID_Usuario);
    }
    public bool Eliminar(int id, UsuarioSesion sesion)
    {
        PermisosNegocio.Exigir(sesion, "Gerente");
        if (id <= 0 || id == sesion.ID_Usuario)
            throw new ArgumentException("Seleccione otro usuario; no puede eliminar su propia sesión.");
        return usuarioDatos.Eliminar(id, sesion.ID_Usuario);
    }
}
