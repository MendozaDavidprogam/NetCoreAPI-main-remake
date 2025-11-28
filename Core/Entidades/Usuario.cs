//NetCoreAPI-main/Core/Entidades/Usuario.cs
namespace Core.Entidades
{
    public class Usuario
    {
        public int Id { get; set; } // autogenerado
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; } // Almacenar hashed
        public string Status { get; set; }
    }
}
