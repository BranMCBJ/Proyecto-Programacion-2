using Microsoft.EntityFrameworkCore;

namespace Proyecto_Periodo_2.Datos
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Models.Usuario> Usuarios { get; set; }
        public DbSet<Models.Cliente> Clientes { get; set; }
        public DbSet<Models.EstadoPrestamo> EstadoPrestamos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Models.Cliente>().ToTable("Clientes");
            modelBuilder.Entity<Models.EstadoPrestamo>().ToTable("EstadoPrestamos");
        }
    }
}
