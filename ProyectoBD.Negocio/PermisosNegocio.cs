using ProyectoBD.Entidades;
namespace ProyectoBD.Negocio;

public static class PermisosNegocio
{
    public static void Exigir(UsuarioSesion sesion, params string[] roles)
    {
        if (sesion.ID_Usuario <= 0 || !roles.Contains(sesion.NombreRol))
            throw new InvalidOperationException("Su rol no tiene permiso para esta operación.");
    }
}
