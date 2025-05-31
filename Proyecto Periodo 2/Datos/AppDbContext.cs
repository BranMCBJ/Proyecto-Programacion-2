using Microsoft.EntityFrameworkCore;
using Proyecto_Periodo_2.Models;

namespace Proyecto_Periodo_2.Datos
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<EstadoPrestamo> EstadoPrestamos { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<CopiaLibro> CopiasLibros { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<EstadoCopiaLibro> EstadoCopiaLibro { get; set; }
        public DbSet<Prestamo> Prestamo { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");
            modelBuilder.Entity<EstadoPrestamo>().ToTable("EstadoPrestamos");
            modelBuilder.Entity<Libro>().ToTable("Libros");
            modelBuilder.Entity<CopiaLibro>().ToTable("CopiaLibros");
            modelBuilder.Entity<Stock>().ToTable("Stocks");
            modelBuilder.Entity<Prestamo>().ToTable("Prestamo");
            modelBuilder.Entity<EstadoCopiaLibro>().ToTable("EstadoCopiaLibro");

        }
    }
}
