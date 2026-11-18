using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using ProyectoBD.Entidades;
using System.Data;

namespace ProyectoBD.Datos
{
    public class RolDatos
    {
        public List<Rol> Listar()
        {
            List<Rol> lista = new List<Rol>();

            ConexionBD conexionBD = new ConexionBD();

            using (SqlConnection conexion = conexionBD.CrearConexion())
            {
                conexion.Open();

                string consulta = "SELECT ID_Rol, NombreRol FROM dbo.tbl_Rol ORDER BY ID_Rol";

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            lista.Add(new Rol
                            {
                                ID_Rol = Convert.ToInt32(lector["ID_Rol"]),
                                NombreRol = lector["NombreRol"].ToString() ?? ""
                            });
                        }
                    }
                }
            }

            return lista;

        }

        public bool Insertar(Rol rol)
        {
            ConexionBD conexionBD = new ConexionBD();

            using (SqlConnection conexion = conexionBD.CrearConexion())
            {
                conexion.Open();
                string consulta = @"INSERT INTO dbo.tbl_Rol (NombreRol) VALUES (@NombreRol)";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.Add("@NombreRol", SqlDbType.NVarChar, 45).Value = rol.NombreRol;
                    try
                    {
                        return comando.ExecuteNonQuery() > 0;
                    }
                    catch (SqlException ex) when (ex.Number == 547) { throw new InvalidOperationException("No se puede eliminar el rol porque está asignado a uno o más usuarios.", ex); }
                    catch (SqlException ex) when (ex.Number is 2601 or 2627) { throw new InvalidOperationException("Ya existe un rol con ese nombre.", ex); }
                }
            }
        }

        public bool Actualizar(Rol rol)
        {
            ConexionBD conexionBD = new ConexionBD();
            using (SqlConnection conexion = conexionBD.CrearConexion())
            {
                conexion.Open();
                string consulta = @"UPDATE dbo.tbl_Rol SET NombreRol = @NombreRol WHERE ID_Rol = @ID_Rol";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.Add("@NombreRol", SqlDbType.NVarChar, 45).Value = rol.NombreRol;
                    comando.Parameters.Add("@ID_Rol", SqlDbType.Int).Value = rol.ID_Rol;
                    try
                    {
                        return comando.ExecuteNonQuery() > 0;
                    }
                    catch (SqlException ex) when (ex.Number == 547) { throw new InvalidOperationException("No se puede eliminar el rol porque está asignado a uno o más usuarios.", ex); }
                    catch (SqlException ex) when (ex.Number is 2601 or 2627) { throw new InvalidOperationException("Ya existe un rol con ese nombre.", ex); }
                }
            }
        }

        public bool Eliminar(int idRol)
        {
            ConexionBD conexionBD = new ConexionBD();
            using (SqlConnection conexion = conexionBD.CrearConexion())
            {
                conexion.Open();
                string consulta = @"DELETE FROM dbo.tbl_Rol WHERE ID_Rol = @ID_Rol";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.Add("@ID_Rol", SqlDbType.Int).Value = idRol;
                    try
                    {
                        return comando.ExecuteNonQuery() > 0;
                    }
                    catch (SqlException ex) when (ex.Number == 547) { throw new InvalidOperationException("No se puede eliminar el rol porque está asignado a uno o más usuarios.", ex); }
                    catch (SqlException ex) when (ex.Number is 2601 or 2627) { throw new InvalidOperationException("Ya existe un rol con ese nombre.", ex); }
                }
            }
        }
    }
}
