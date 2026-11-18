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
    public partial class UsuarioDatos
    {
        public Usuario? BuscarPorNombreUsuario(string nombreUsuario)
        {
            Usuario? usuario = null;

            ConexionBD conexionDB = new ConexionBD();

            using (SqlConnection conexion = conexionDB.CrearConexion())
            {
                conexion.Open();

                string consulta = @"
                    SELECT
                        U.ID_Usuario,
                        U.Nombres,
                        U.Apellidos,
                        U.NombreUsuario,
                        U.PasswordHash,
                        U.ID_Rol,
                        R.NombreRol
                    FROM dbo.tbl_Usuario AS U
                    INNER JOIN dbo.tbl_Rol AS R
                        ON U.ID_Rol = R.ID_Rol
                    WHERE U.NombreUsuario = @NombreUsuario;
                ";

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.Add(
                        "@NombreUsuario",
                        SqlDbType.NVarChar,
                        45
                    ).Value = nombreUsuario;

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        if (lector.Read())
                        {
                            usuario = new Usuario
                            {
                                ID_Usuario = Convert.ToInt32(
                                    lector["ID_Usuario"]
                                ),

                                Nombres = lector["Nombres"].ToString() ?? "",

                                Apellidos = lector["Apellidos"].ToString() ?? "",

                                NombreUsuario =
                                    lector["NombreUsuario"].ToString() ?? "",

                                PasswordHash =
                                    lector["PasswordHash"].ToString() ?? "",

                                ID_Rol = Convert.ToInt32(
                                    lector["ID_Rol"]
                                ),

                                NombreRol =
                                    lector["NombreRol"].ToString() ?? ""
                            };
                        }
                    }
                }
            }

            return usuario;
        }


        public bool CrearUsuario(Usuario usuario)
        {
            ConexionBD conexionDB = new ConexionBD();

            using (SqlConnection conexion = conexionDB.CrearConexion())
            {
                conexion.Open();

                string consulta = @"
            INSERT INTO dbo.tbl_Usuario
            (
                Nombres,
                Apellidos,
                NombreUsuario,
                PasswordHash,
                ID_Rol
            )
            VALUES
            (
                @Nombres,
                @Apellidos,
                @NombreUsuario,
                @PasswordHash,
                @ID_Rol
            );
        ";

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.Add("@Nombres", SqlDbType.NVarChar, 45)
                        .Value = usuario.Nombres;

                    comando.Parameters.Add("@Apellidos", SqlDbType.NVarChar, 45)
                        .Value = usuario.Apellidos;

                    comando.Parameters.Add("@NombreUsuario", SqlDbType.NVarChar, 45)
                        .Value = usuario.NombreUsuario;

                    comando.Parameters.Add("@PasswordHash", SqlDbType.NVarChar, 255)
                        .Value = usuario.PasswordHash;

                    comando.Parameters.Add("@ID_Rol", SqlDbType.Int)
                        .Value = usuario.ID_Rol;

                    return comando.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
