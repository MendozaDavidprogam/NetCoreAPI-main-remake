//NetCoreAPI-main/Infrastructure/Repositorios/UsuarioRepositorio.cs

using Core.Entidades;
using Core.Interfaces.Repositorios;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositorios
{
    public class UsuarioRepositorio : BaseRepositorio<Usuario>, IUsuarioRepositorio
    {
        public UsuarioRepositorio(AppDbContext context) : base(context)
        {
        }

        public async ValueTask<Usuario> ConsultarPorEmail(string email)
        {
            return await dbSet.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async ValueTask<Usuario> IniciarSesion(string email, string password)
        {
            // No comparar contraseñas en claro en repo; el servicio hará el check.
            return await dbSet.Where(u => u.Email == email).FirstOrDefaultAsync();
        }
    }
}
