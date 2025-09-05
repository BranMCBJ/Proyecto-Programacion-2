using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;

namespace Proyecto_Periodo_2.Controllers
{
    public class LibroController : Controller
    {
        #region Propiedades y Constructor
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LibroController(AppDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }
        #endregion

        #region Acciones de Lectura
        // GET: Libro
        public IActionResult Index()
        {
            try
            {
                var libros = _db.Libros.Where(c => c.Activo == true).ToList();
                return View(libros);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<LibroVM>());
            }
        }
        #endregion

        #region Vistas Parciales
        // GET: Libro/Create
        [HttpGet]
        public ActionResult _PartialCrearLibro()
        {

            var libroVM = new LibroVM();
            return PartialView("_PartialCrearLibro", libroVM);
        }

        [HttpGet]
        public ActionResult _PartialVerMasLibro(int? id)
        {
            try
            {
                if (id == null)
                {
                    TempData["Error"] = "ID del libro no válido.";
                    return RedirectToAction(nameof(Index));
                }

                //saca el libro por el id
                var libro = _db.Libros.FirstOrDefault(s =>
                    s.IdLibro == id &&
                    s.Activo == true);

                if (libro == null)
                {
                    TempData["Error"] = "Libro no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                var libroVM = new LibroVM()
                {
                    Libro = libro,
                    Imagen = null
                };

                return PartialView("_PartialVerMasLibro", libroVM);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                ViewBag.Error = ex.Message;
                TempData["Error"] = "Error al cargar el libro.";
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion

        #region Acciones de Creación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearLibro(LibroVM libroVM)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));

            if (libroVM.Libro == null)
            {
                TempData["Error"] = "Datos del libro inválidos.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var imagen = libroVM.Imagen;
                var libro = libroVM.Libro;

                if (imagen != null && imagen.Length > 0)
                {
                    // Validar extensiones permitidas
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    // Obtener extension del archivo en minusculas
                    var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();
                    
                    // Verificar que la extension este en lista de permitidas
                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["Error"] = "Solo se permiten archivos de imagen (jpg jpeg png gif bmp).";
                        return RedirectToAction(nameof(Index));
                    }

                    // Validar tamano maximo de 5MB
                    if (imagen.Length > 5 * 1024 * 1024)
                    {
                        TempData["Error"] = "El archivo no puede superar los 5MB.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Construir ruta de carpeta para imagenes de libros
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Libros");
                    // Crear carpeta si no existe
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Generar nombre unico para evitar conflictos
                    var fileName = Guid.NewGuid().ToString() + extension;
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Crear stream y copiar archivo al servidor
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imagen.CopyTo(stream);
                    }

                    // Guardar solo el nombre del archivo en la base de datos
                    libro.ImagenUrl = fileName;
                }

                // Marcar libro como activo
                libro.Activo = true;

                // Paso 1: Agregar libro a contexto y guardar para obtener ID generado
                _db.Libros.Add(libro);
                _db.SaveChanges();

                // Obtener ID del libro recien insertado
                var libroId = libro.IdLibro;

                // Paso 2: Crear copias fisicas del libro usando el ID obtenido
                if (libro.Stock > 0)
                {
                    CrearCopiasLibro(libro.Stock, libroId);
                    _db.SaveChanges(); 
                }

                TempData["Success"] = "Libro creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear el libro: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }
        #endregion

        #region Acciones de Edición y Eliminación
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpDelete(LibroVM libroVM, string accion)
        {
            // Validar que el ViewModel y el libro no sean null
            if (libroVM?.Libro == null)
            {
                TempData["Error"] = "Datos del libro inválidos.";
                return RedirectToAction(nameof(Index));
            }

            if (accion == "actualizar")
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Por favor, corrija los errores en el formulario.";
                    return RedirectToAction(nameof(Index));
                }

                var libroEnDb = _db.Libros.FirstOrDefault(x => x.IdLibro == libroVM.Libro.IdLibro);
                if (libroEnDb == null)
                {
                    TempData["Error"] = "Libro no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Actualizar campos
                libroEnDb.Titulo = libroVM.Libro.Titulo;
                libroEnDb.ISBN = libroVM.Libro.ISBN;
                libroEnDb.FechaPublicacion = libroVM.Libro.FechaPublicacion;
                libroEnDb.ClasificacionEdad = libroVM.Libro.ClasificacionEdad;
                libroEnDb.Stock = libroVM.Libro.Stock;
                libroEnDb.Descripcion = libroVM.Libro.Descripcion;

                var imagen = libroVM.Imagen;
                if (imagen != null && imagen.Length > 0)
                {
                    // Validar extensiones permitidas
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    var extension = Path.GetExtension(imagen.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["Error"] = "Solo se permiten archivos de imagen (jpg, jpeg, png, gif, bmp).";
                        return RedirectToAction(nameof(Index));
                    }

                    if (imagen.Length > 5 * 1024 * 1024)
                    {
                        TempData["Error"] = "El archivo no puede superar los 5MB.";
                        return RedirectToAction(nameof(Index));
                    }

                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Libros");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // Eliminar imagen anterior si existe
                    if (!string.IsNullOrEmpty(libroEnDb.ImagenUrl))
                    {
                        var oldImagePath = Path.Combine(uploadsFolder, libroEnDb.ImagenUrl);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    var fileName = Guid.NewGuid().ToString() + extension;
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imagen.CopyTo(stream);
                    }

                    libroEnDb.ImagenUrl = fileName; // Solo guardar el nombre del archivo
                }

                _db.Libros.Update(libroEnDb);
                _db.SaveChanges();
                TempData["Success"] = "Libro actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            if (accion == "eliminar")
            {
                var libroEnDb = _db.Libros.FirstOrDefault(x => x.IdLibro == libroVM.Libro.IdLibro);
                if (libroEnDb == null || libroEnDb.Activo == false)
                {
                    TempData["Error"] = "Libro no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Eliminar la imagen física si existe
                if (!string.IsNullOrEmpty(libroEnDb.ImagenUrl))
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Images", "Libros");
                    var imagePath = Path.Combine(uploadsFolder, libroEnDb.ImagenUrl);
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                libroEnDb.Activo = false;
                _db.Libros.Update(libroEnDb);
                _db.SaveChanges();
                EliminarCopiaLibro(libroEnDb.IdLibro);
                TempData["Success"] = "Libro eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Acción no válida.";
            return RedirectToAction(nameof(Index));
        }
        #endregion

        #region Métodos de copias de libro
        public void CrearCopiasLibro(int? stock, int? idLibro)
        {
            if (stock <= 0) return;

            var lista = new List<CopiaLibro>();
            for (var i = 0; i < stock; i++)
            {
                lista.Add(new CopiaLibro
                {
                    IdLibro = idLibro,
                    IdEstadoCopiaLibro = 1, //Disponible
                    Activo = true
                });
            }

            _db.CopiasLibros.AddRange(lista);
            _db.SaveChanges();
        }

        public void EliminarCopiaLibro(int? id)
        {
            var copias = _db.CopiasLibros.Where(c => c.IdLibro == id).ToList();
            foreach (var item in copias)
            {
                item.Activo = false;
            }
            _db.CopiasLibros.UpdateRange(copias);
            _db.SaveChanges();
        }
        #endregion
    }
}