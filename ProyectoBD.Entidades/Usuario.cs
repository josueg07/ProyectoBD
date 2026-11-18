namespace ProyectoBD.Entidades
{
    public class Usuario
    {
        public int ID_Usuario
        {
            get; set;
        }
        public string Nombres { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
        public string NombreCompleto => $"{Nombres} {Apellidos}".Trim();
        public string PasswordHash { get; set; } = string.Empty;
        public int ID_Rol
        {
            get; set;
        }
        public string NombreRol { get; set; } = string.Empty;
    }
}
