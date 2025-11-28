//NetCoreAPI-main/Core/Interfaces/IUnidadDeTrabajo.cs

using Core.Interfaces.Repositorios;

namespace Core.Interfaces
{
    public interface IUnidadDeTrabajo : IDisposable
    {
        IUsuarioRepositorio UsuarioRepositorio { get; }
        Task<int> CommitAsync();
    }
}
