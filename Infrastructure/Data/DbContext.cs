//NetCoreAPI-main/Infrastructure/Data/DbContext.cs

using Core.Entidades;
using Infrastructure.Data.Configuraciones;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Usuario> Usuarios { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new UsuarioConfiguracion());
            base.OnModelCreating(builder);
        }
    }
}
