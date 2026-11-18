using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoBD.Entidades;
using ProyectoBD.Datos;

namespace ProyectoBD.Negocio
{
    public partial class UsuarioNegocio
    {
        private readonly UsuarioDatos usuarioDatos;

        public UsuarioNegocio()
        {
            usuarioDatos = new UsuarioDatos();
        }

        public UsuarioSesion? IniciarSesion(
            string nombreUsuario,
            string password)
        {
            if (string.IsNullOrWhiteSpace(nombreUsuario) ||
                string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            Usuario? usuario =
                usuarioDatos.BuscarPorNombreUsuario(nombreUsuario.Trim());

            if (usuario == null)
            {
                return null;
            }

            bool passwordCorrecta =
                BCrypt.Net.BCrypt.Verify(
                    password,
                    usuario.PasswordHash
                );

            if (!passwordCorrecta)
            {
                return null;
            }

            UsuarioSesion sesion = new UsuarioSesion
            {
                ID_Usuario = usuario.ID_Usuario,

                NombreCompleto =
                    $"{usuario.Nombres} {usuario.Apellidos}",

                NombreUsuario = usuario.NombreUsuario,

                ID_Rol = usuario.ID_Rol,

                NombreRol = usuario.NombreRol
            };

            return sesion;
        }


        public bool CrearUsuarioInicial(string nombres, string apellidos, string nombreUsuario, string password, int idRol)
        {
            if (string.IsNullOrWhiteSpace(nombres) ||
                string.IsNullOrWhiteSpace(apellidos) ||
                string.IsNullOrWhiteSpace(nombreUsuario) ||
                string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            Usuario usuario = new Usuario
            {
                Nombres = nombres.Trim(),
                Apellidos = apellidos.Trim(),
                NombreUsuario = nombreUsuario.Trim(),
                PasswordHash = hash,
                ID_Rol = idRol
            };

            return usuarioDatos.CrearUsuario(usuario);
        }
    }
}
