//NetCoreAPI-main/Infrastructure/Data/UnidadDeTrabajo.cs

using Core.Interfaces;
using Core.Interfaces.Repositorios;
using Infrastructure.Repositorios;

namespace Infrastructure.Data
{
    public class UnidadDeTrabajo : IUnidadDeTrabajo
    {
        private readonly AppDbContext _context;
        private IUsuarioRepositorio _usuarioRepositorio;

        public UnidadDeTrabajo(AppDbContext context)
        {
            _context = context;
        }

        public IUsuarioRepositorio UsuarioRepositorio => _usuarioRepositorio ??= new UsuarioRepositorio(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
