//NetCoreAPI-main/Infrastructure/Repositorios/BaseRepositorio.cs

using Core.Interfaces.Repositorios;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositorios
{
    public class BaseRepositorio<Entidad> : IBaseRepositorio<Entidad> where Entidad : class
    {
        internal AppDbContext Context;
        internal DbSet<Entidad> dbSet;

        public BaseRepositorio(AppDbContext context)
        {
            this.Context = context;
            this.dbSet = context.Set<Entidad>();
        }

        public virtual async ValueTask<Entidad> ObtenerPorIdAsincrono(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<Entidad>> ObtenerTodosAsincrono()
        {
            return await dbSet.ToListAsync();
        }

        public virtual void Remover(Entidad entidad)
        {
            dbSet.Remove(entidad);
        }

        public virtual void RemoverRango(IEnumerable<Entidad> entidades)
        {
            dbSet.RemoveRange(entidades);
        }

        public virtual async Task Actualizar(Entidad entidad)
        {
            dbSet.Attach(entidad);
            Context.Entry(entidad).State = EntityState.Modified;
        }

        public virtual async Task<Entidad> AgregarAsincrono(Entidad entidad)
        {
            var entityEntry = await dbSet.AddAsync(entidad);
            return entityEntry.Entity;
        }
    }
}
