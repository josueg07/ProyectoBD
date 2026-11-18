using ProyectoBD.Entidades;
using System.Security.Claims;

namespace ProyectoBD.Web.Helpers
{
    public static class SessionExtensions
    {
        public static UsuarioSesion ObtenerSesion(this ClaimsPrincipal usuario)
        {
            int.TryParse(
                usuario.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                out int idUsuario
            );

            int.TryParse(
                usuario.FindFirst("ID_Rol")?.Value,
                out int idRol
            );

            return new UsuarioSesion
            {
                ID_Usuario = idUsuario,

                NombreCompleto =
                    usuario.Identity?.Name ?? string.Empty,

                NombreUsuario =
                    usuario.FindFirst("NombreUsuario")?.Value
                    ?? string.Empty,

                ID_Rol = idRol,

                NombreRol =
                    usuario.FindFirst(ClaimTypes.Role)?.Value
                    ?? string.Empty
            };
        }
    }
}
