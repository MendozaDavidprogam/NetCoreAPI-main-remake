//NetCoreAPI-main/Core/Entidades/Usuario.cs
using System.Text.Json.Serialization;

namespace Core.Entidades
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; }

        public string Status { get; set; }
    }
}
