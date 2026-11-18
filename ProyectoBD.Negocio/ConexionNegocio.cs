using ProyectoBD.Datos;

namespace ProyectoBD.Negocio
{
    public class ConexionNegocio
    {
         public bool ProbarConexion()
        {
            ConexionBD conexionDB = new ConexionBD();

            return conexionDB.ProbarConexion();
        }
    }
}
