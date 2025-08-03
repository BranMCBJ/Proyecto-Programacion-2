using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Data
{
    public class AppDbContext : IdentityDbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<EstadoPrestamo> EstadoPrestamo { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<CopiaLibro> CopiasLibros { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<EstadoCopiaLibro> EstadoCopiaLibro { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<CopiaLibroPrestamo> CopiasLibrosPrestamos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");
            modelBuilder.Entity<EstadoPrestamo>().ToTable("EstadoPrestamo");
            modelBuilder.Entity<Libro>().ToTable("Libros");
            modelBuilder.Entity<CopiaLibro>().ToTable("CopiaLibros");
            modelBuilder.Entity<Stock>().ToTable("Stocks");
            modelBuilder.Entity<Prestamo>().ToTable("Prestamo");
            modelBuilder.Entity<EstadoCopiaLibro>().ToTable("EstadoCopiaLibro");

            base.OnModelCreating(modelBuilder);
        }
    }
}
