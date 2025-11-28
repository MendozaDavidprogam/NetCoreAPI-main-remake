//NetCoreAPI-main/Core/Interfaces/Servicios/IUsuarioServicio.cs

using Core.Entidades;
using Core.Respuestas;

namespace Core.Interfaces.Servicios
{
    public interface IUsuarioServicio : IBaseServicio<Usuario>
    {
        Task<Respuesta<RespuestaIniciarSesion>> IniciarSesion(string email, string password);
        Task<Respuesta<ModeloRegistrarse>> Registrarse(ModeloRegistrarse modeloRegistrarse);
    }
}
