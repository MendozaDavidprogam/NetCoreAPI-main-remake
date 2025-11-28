//NetCoreAPI-main/Core/Interfaces/Repositorios/IUsuarioRepositorio.cs

using Core.Entidades;

namespace Core.Interfaces.Repositorios
{
    public interface IUsuarioRepositorio : IBaseRepositorio<Usuario>
    {
        ValueTask<Usuario> IniciarSesion(string email, string password);
        ValueTask<Usuario> ConsultarPorEmail(string email);
    }
}
