using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProyectoBD.Datos;
using ProyectoBD.Entidades;

namespace ProyectoBD.Negocio
{
    public class RolNegocio
    {
        private readonly RolDatos rolDatos;

        public RolNegocio()
        {
            rolDatos = new RolDatos();
        }

        public List<Rol> ListarRoles()
        {
            return rolDatos.Listar();
        }

        public bool Guardar(Rol rol)
        {
            if ((string.IsNullOrWhiteSpace(rol.NombreRol) || rol.NombreRol.Trim().Length > 45))
            {
                return false;
            }
            rol.NombreRol = rol.NombreRol.Trim();
            return rolDatos.Insertar(rol);
        }

        public bool Actualizar(Rol rol)
        {
            if (rol.ID_Rol <= 0 || (string.IsNullOrWhiteSpace(rol.NombreRol) || rol.NombreRol.Trim().Length > 45))
            {
                return false;
            }
            rol.NombreRol = rol.NombreRol.Trim();
            return rolDatos.Actualizar(rol);
        }

        public bool Eliminar(int idRol)
        {
            if (idRol <= 0)
            {
                return false;
            }
            return rolDatos.Eliminar(idRol);
        }
    }
}
