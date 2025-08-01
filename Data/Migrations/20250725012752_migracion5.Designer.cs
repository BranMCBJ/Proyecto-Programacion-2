﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Data;

#nullable disable

namespace Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250725012752_migracion5")]
    partial class migracion5
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Proyecto_Periodo_2.Models.Cliente", b =>
                {
                    b.Property<int>("IdCliente")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCliente"));

                    b.Property<bool?>("Activo")
                        .HasColumnType("bit");

                    b.Property<string>("Apellido1")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Apellido2")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<int?>("CantidadPrestamosDisponibles")
                        .HasColumnType("int");

                    b.Property<string>("Cedula")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Correo")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Nombre")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Telefono")
                        .HasMaxLength(15)
                        .HasColumnType("nvarchar(15)");

                    b.HasKey("IdCliente");

                    b.ToTable("Clientes", (string)null);
                });

            modelBuilder.Entity("Proyecto_Periodo_2.Models.CopiaLibro", b =>
                {
                    b.Property<int>("IdCopiaLibro")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdCopiaLibro"));

                    b.Property<bool?>("Activo")
                        .HasColumnType("bit");

                    b.Property<int>("IdEstadoCopiaLibro")
                        .HasColumnType("int");

                    b.Property<int>("IdLibro")
                        .HasColumnType("int");

                    b.HasKey("IdCopiaLibro");

                    b.HasIndex("IdEstadoCopiaLibro");

                    b.HasIndex("IdLibro");

                    b.ToTable("CopiaLibros", (string)null);
                });

            modelBuilder.Entity("Proyecto_Periodo_2.Models.CopiaLibroPrestamo", b =>
                {
                    b.Property<int>("IdRelacion")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdRelacion"));

                    b.Property<bool>("Activo")
                        .HasColumnType("bit");

                    b.Property<int?>("CopiaLibroIdCopiaLibro")
                        .HasColumnType("int");

                    b.Property<int>("IdCopiaLibro")
                        .HasColumnType("int");

                    b.Property<int>("IdPrestamo")
                        .HasColumnType("int");

                    b.Property<int?>("PrestamoIdPrestamo")
                        .HasColumnType("int");

                    b.HasKey("IdRelacion");

                    b.HasIndex("CopiaLibroIdCopiaLibro");

                    b.HasIndex("PrestamoIdPrestamo");

                    b.ToTable("CopiasLibrosPrestamos");
                });

            modelBuilder.Entity("Proyecto_Periodo_2.Models.EstadoCopiaLibro", b =>
                {
                    b.Property<int>("IdEstadoCopialibro")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdEstadoCopialibro"));

                    b.Property<bool?>("Activo")
                        .HasColumnType("bit");

                    b.Property<string>("Descripcion")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("Nombre")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("IdEstadoCopialibro");

                    b.ToTable("EstadoCopiaLibro", (string)null);
                });

            modelBuilder.Entity("Proyecto_Periodo_2.Models.EstadoPrestamo", b =>
                {
                    b.Property<int>("_IdEstado")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("_IdEstado"));

                    b.Property<bool?>("_Activo")
                        .HasColumnType("bit");

                    b.Property<string>("_Descripcion")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<string>("_Nombre")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.HasKey("_IdEstado");

                    b.ToTable("EstadoPrestamo", (string)null);
                });

            modelBuilder.Entity("Proyecto_Periodo_2.Models.Libro", b =>
                {
                    b.Property<int>("IdLibro")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdLibro"));

                    b.Property<bool?>("Activo")
                        .HasColumnType("bit");

                    b.Property<int>("ClasificacionEdad")
                        .HasColumnType("int");

                    b.Property<string>("Descripcion")
                        .IsRequired()
                        .HasMaxLength(400)
                        .HasColumnType("nvarchar(400)");

                    b.Property<DateTime>("FechaPublicacion")
                        .HasColumnType("datetime2");

                    b.Property<string>("ISBN")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("nvarchar(13)");

                    b.Property<int>("IdStock")
                        .HasColumnType("int");

                    b.Property<string>("Titulo")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("IdLibro");

                    b.HasIndex("IdStock");

                    b.ToTable("Libros", (string)null);
                });

            modelBuilder.Entity("Proyecto_Periodo_2.Models.Prestamo", b =>
                {
                    b.Property<int>("IdPrestamo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdPrestamo"));

                    b.Property<bool?>("Activo")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("FechaInicio")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("FechaLimite")
                        .HasColumnType("datetime2");

                    b.Property<int?>("IdCliente")
                        .HasColumnType("int");

                    b.Property<int?>("IdEstadoPrestamo")
                        .HasColumnType("int");

                    b.Property<int?>("IdUsuario")
                        .HasColumnType("int");

                    b.HasKey("IdPrestamo");

                    b.HasIndex("IdCliente");

                    b.HasIndex("IdEstadoPrestamo");

                    b.HasIndex("IdUsuario");

                    b.ToTable("Prestamo", (string)null);
                });

            modelBuilder.Entity("Proyecto_Periodo_2.Models.Stock", b =>
                {
                    b.Property<int>("IdStock")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdStock"));

                    b.Property<bool?>("Activo")
                        .HasColumnType("bit");

                    b.Property<int>("Cantidad")
                        .HasColumnType("int");

                    b.HasKey("IdStock");

                    b.ToTable("Stocks", (string)null);
                });

            modelBuilder.Entity("Proyecto_Periodo_2.Models.Usuario", b =>
                {
                    b.Property<int>("IdUsuario")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdUsuario"));

                    b.Property<bool?>("Activo")
                        .HasColumnType("bit");

                    b.Property<string>("Apellido1")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Apellido2")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("Cedula")
                        .HasMaxLength(10)
                        .HasColumnType("nvarchar(10)");

                    b.Property<string>("Contrasena")
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)");

                    b.Property<string>("Nombre")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<string>("NombreDeUsuario")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("IdUsuario");

                    b.ToTable("Usuarios", (string)null);
                });

            modelBuilder.Entity("Proyecto_Periodo_2.Models.CopiaLibro", b =>
                {
                    b.HasOne("Proyecto_Periodo_2.Models.EstadoCopiaLibro", "EstadoCopiaLibro")
                        .WithMany()
                        .HasForeignKey("IdEstadoCopiaLibro")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Proyecto_Periodo_2.Models.Libro", "Libro")
                        .WithMany()
                        .HasForeignKey("IdLibro")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EstadoCopiaLibro");

                    b.Navigation("Libro");
                });

            modelBuilder.Entity("Proyecto_Periodo_2.Models.CopiaLibroPrestamo", b =>
                {
                    b.HasOne("Proyecto_Periodo_2.Models.CopiaLibro", "CopiaLibro")
                        .WithMany()
                        .HasForeignKey("CopiaLibroIdCopiaLibro");

                    b.HasOne("Proyecto_Periodo_2.Models.Prestamo", "Prestamo")
                        .WithMany()
                        .HasForeignKey("PrestamoIdPrestamo");

                    b.Navigation("CopiaLibro");

                    b.Navigation("Prestamo");
                });

            modelBuilder.Entity("Proyecto_Periodo_2.Models.Libro", b =>
                {
                    b.HasOne("Proyecto_Periodo_2.Models.Stock", "Stock")
                        .WithMany()
                        .HasForeignKey("IdStock")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Stock");
                });

            modelBuilder.Entity("Proyecto_Periodo_2.Models.Prestamo", b =>
                {
                    b.HasOne("Proyecto_Periodo_2.Models.Cliente", "cliente")
                        .WithMany()
                        .HasForeignKey("IdCliente");

                    b.HasOne("Proyecto_Periodo_2.Models.EstadoPrestamo", "estadoPrestamo")
                        .WithMany()
                        .HasForeignKey("IdEstadoPrestamo");

                    b.HasOne("Proyecto_Periodo_2.Models.Usuario", "usuario")
                        .WithMany()
                        .HasForeignKey("IdUsuario");

                    b.Navigation("cliente");

                    b.Navigation("estadoPrestamo");

                    b.Navigation("usuario");
                });
#pragma warning restore 612, 618
        }
    }
}
