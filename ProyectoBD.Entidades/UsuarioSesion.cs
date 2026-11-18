using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoBD.Entidades
{
    public class UsuarioSesion
    {
            public int ID_Usuario { get; set; }
            public string NombreCompleto { get; set; } = string.Empty;
            public string NombreUsuario { get; set; } = string.Empty;
            public int ID_Rol { get; set; }
            public string NombreRol { get; set; } = string.Empty;
    }
}
