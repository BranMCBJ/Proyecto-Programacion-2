using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Security.Claims;

namespace Data
{
    /// <summary>
    /// Contexto de base de datos principal de la aplicación
    /// Hereda de IdentityDbContext para incluir las tablas de Identity
    /// </summary>
    public class AppDbContext : IdentityDbContext
    {
        /// <summary>
        /// Constructor del contexto de base de datos
        /// </summary>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        // Definición de las tablas de la base de datos
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<EstadoPrestamo> EstadoPrestamo { get; set; }
        public DbSet<Libro> Libros { get; set; }
        public DbSet<CopiaLibro> CopiasLibros { get; set; }
        public DbSet<EstadoCopiaLibro> EstadoCopiaLibro { get; set; }
        public DbSet<Prestamo> Prestamos { get; set; }
        public DbSet<CopiaLibroPrestamo> CopiasLibrosPrestamos { get; set; }

        /// <summary>
        /// Configuración del modelo de base de datos y datos iniciales
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuración de nombres de tablas
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Cliente>().ToTable("Clientes");
            modelBuilder.Entity<EstadoPrestamo>().ToTable("EstadoPrestamo");
            modelBuilder.Entity<Libro>().ToTable("Libros");
            modelBuilder.Entity<CopiaLibro>().ToTable("CopiaLibros");
            modelBuilder.Entity<Prestamo>().ToTable("Prestamo");
            modelBuilder.Entity<EstadoCopiaLibro>().ToTable("EstadoCopiaLibro");

            // Llamar al método base para configurar Identity
            base.OnModelCreating(modelBuilder);

            // Datos iniciales para estados de copia de libro
            modelBuilder.Entity<EstadoCopiaLibro>().HasData(
                new EstadoCopiaLibro { IdEstadoCopialibro = 1, Nombre = "Disponible", Activo = true, Descripcion = "La copia del libro se puede prestar" },
                new EstadoCopiaLibro { IdEstadoCopialibro = 2, Nombre = "Prestado", Activo = true, Descripcion = "La copia del libro esta en un prestamo" },
                new EstadoCopiaLibro { IdEstadoCopialibro = 3, Nombre = "Dañado", Activo = true, Descripcion = "La copia del libro esta dañada" }
            );

            // Datos iniciales para estados de préstamo
            modelBuilder.Entity<EstadoPrestamo>().HasData(
                new EstadoPrestamo { IdEstado = 1, Nombre = "Vigente", Activo = true, Descripcion = "El prestamo sigue en vigencia" },
                new EstadoPrestamo { IdEstado = 2, Nombre = "Devuelto", Activo = true, Descripcion = "El prestamo ya termino" }
            );

            // Configuración de roles del sistema
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "1", Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = "2", Name = "Usuario", NormalizedName = "USUARIO" }
            );

            // Usuario seed
            var usuarioSeed = new Usuario
            {
                Id = "3",
                Nombre = "Julian",
                Apellido1 = "Ceciliano",
                Apellido2 = "Picado",
                Cedula = "305760805",
                Email = "cecilianojulian64@gmail.com",
                NormalizedEmail = "CECILIANOJULIAN64@GMAIL.COM",
                UserName = "Julai",
                NormalizedUserName = "JULAI",
                NombreUsuario = "Julai",
                PhoneNumber = "12345678",
                UrlImagen = "/Usuario/Imagenes/d724626d-b41f-47d7-acec-8b85fe3f8de5.jpg",
                Activo = true,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            };

            var hasher = new PasswordHasher<Usuario>();
            usuarioSeed.PasswordHash = hasher.HashPassword(usuarioSeed, "123456");

            modelBuilder.Entity<Usuario>().HasData(usuarioSeed);

            modelBuilder.Entity<IdentityUserClaim<string>>().HasData(
                new IdentityUserClaim<string>
                {
                    Id = 1,
                    UserId = "3",
                    ClaimType = ClaimTypes.Role,
                    ClaimValue = "Admin"
                },
                new IdentityUserClaim<string>
                {
                    Id = 2,
                    UserId = "3",
                    ClaimType = "Nombre",
                    ClaimValue = "Julian"
                },
                new IdentityUserClaim<string>
                {
                    Id = 3,
                    UserId = "3",
                    ClaimType = "NombreUsuario",
                    ClaimValue = "Julai"
                },
                new IdentityUserClaim<string>
                {
                    Id = 4,
                    UserId = "3",
                    ClaimType = "Apellido1",
                    ClaimValue = "Ceciliano"
                },
                new IdentityUserClaim<string>
                {
                    Id = 5,
                    UserId = "3",
                    ClaimType = "Apellido2",
                    ClaimValue = "Picado"
                },
                new IdentityUserClaim<string>
                {
                    Id = 6,
                    UserId = "3",
                    ClaimType = "Cedula",
                    ClaimValue = "305760805"
                },
                new IdentityUserClaim<string>
                {
                    Id = 7,
                    UserId = "3",
                    ClaimType = "UrlImagen",
                    ClaimValue = "/Usuario/Imagenes/Foto perfil.png"
                }
            );

            //Asigna rol al usuario
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(new IdentityUserRole<string>
            {
                RoleId = "1",
                UserId = "3"
            });
        }

    }
}