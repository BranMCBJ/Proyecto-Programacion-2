using Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.IO;
using System.Linq;

namespace Proyecto_Periodo_2.Controllers
{
    public class LibroController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public LibroController(AppDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Libro
        public IActionResult Index()
        {
            try
            {
                var libros = _db.Libros
                    .Where(l => l.Activo == true)
                    .ToList();

                // Calcular stock dinámicamente para cada libro
                foreach (var libro in libros)
                {
                    libro.StockCalculado = _db.CopiasLibros
                        .Count(c => c.IdLibro == libro.IdLibro && c.Activo == true);
                }

                Console.WriteLine($"Libros encontrados: {libros.Count}");
                return View(libros);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Index: {ex.Message}");
                return View(new List<Libro>());
            }
        }

        // GET: Libro/Create
        [HttpGet]
        public ActionResult Create()
        {
            Console.WriteLine("=== MÉTODO GET CREATE EJECUTADO ===");
            return View();
        }

        // POST: Libro/Create - SIN LÓGICA DE STOCK
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Libro libro, IFormFile files)
        {
            try
            {
                Console.WriteLine("=== MÉTODO POST CREATE SIN STOCK ===");

                // Validaciones simples
                if (string.IsNullOrWhiteSpace(libro?.Titulo) || string.IsNullOrWhiteSpace(libro?.ISBN))
                {
                    TempData["Error"] = "Título e ISBN son obligatorios";
                    return RedirectToAction("Index");
                }

                // Verificar duplicados
                if (_db.Libros.Any(l => l.Titulo == libro.Titulo && l.Activo == true))
                {
                    TempData["Error"] = "Ya existe un libro con ese título";
                    return RedirectToAction("Index");
                }

                if (_db.Libros.Any(l => l.ISBN == libro.ISBN && l.Activo == true))
                {
                    TempData["Error"] = "Ya existe un libro con ese ISBN";
                    return RedirectToAction("Index");
                }

                // Crear el libro SIN relación a Stock
                var nuevoLibro = new Libro
                {
                    Titulo = libro.Titulo?.Trim(),
                    ISBN = libro.ISBN?.Trim(),
                    FechaPublicacion = libro.FechaPublicacion,
                    ClasificacionEdad = libro.ClasificacionEdad ?? 0,
                    IdStock = null,
                    Descripcion = string.IsNullOrWhiteSpace(libro.Descripcion) ? "Sin descripción" : libro.Descripcion.Trim(),
                    Activo = true
                };

                // MANEJO DE IMAGEN (mantener código existente)
                if (files != null && files.Length > 0)
                {
                    try
                    {
                        string webRootPath = _webHostEnvironment.WebRootPath;
                        string carpetaFisica = Path.Combine(webRootPath, "images", "libros");

                        if (!Directory.Exists(carpetaFisica))
                        {
                            Directory.CreateDirectory(carpetaFisica);
                        }

                        string extension = Path.GetExtension(files.FileName);
                        string fileName = Guid.NewGuid().ToString() + extension;
                        string rutaFisica = Path.Combine(carpetaFisica, fileName);

                        using (var stream = new FileStream(rutaFisica, FileMode.Create))
                        {
                            files.CopyTo(stream);
                        }

                        nuevoLibro.ImagenUrl = $"/images/libros/{fileName}";
                        Console.WriteLine($"Imagen guardada: {nuevoLibro.ImagenUrl}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error al guardar imagen: {ex.Message}");
                        nuevoLibro.ImagenUrl = "/images/no-image.png";
                    }
                }
                else
                {
                    nuevoLibro.ImagenUrl = "/images/no-image.png";
                }

                _db.Libros.Add(nuevoLibro);



                _db.SaveChanges();

                Console.WriteLine($"Libro creado - ID: {nuevoLibro.IdLibro}");

                TempData["Success"] = "¡Libro creado exitosamente! Ahora puedes agregar copias desde 'Gestionar Copias'.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Create: {ex.Message}");
                TempData["Error"] = "Error al guardar el libro.";
                return RedirectToAction("Index");
            }
        }

        #region Edit

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var libro = _db.Libros.Find(id);
                if (libro == null)
                {
                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Libro libro, IFormFile files)
        {
            try
            {
                // Limpiar ModelState para testing
                ModelState.Clear();

                if (string.IsNullOrWhiteSpace(libro?.Titulo) || string.IsNullOrWhiteSpace(libro?.ISBN))
                {
                    TempData["Error"] = "Título e ISBN son obligatorios";
                    return RedirectToAction("Index");
                }

                var libroExistente = _db.Libros.Find(libro.IdLibro);
                if (libroExistente == null)
                {
                    TempData["Error"] = "Libro no encontrado";
                    return RedirectToAction("Index");
                }

                // Verificar duplicados (excluyendo el libro actual)
                if (_db.Libros.Any(l => l.Titulo == libro.Titulo && l.Activo == true && l.IdLibro != libro.IdLibro))
                {
                    TempData["Error"] = "Ya existe otro libro con ese título";
                    return RedirectToAction("Index");
                }

                if (_db.Libros.Any(l => l.ISBN == libro.ISBN && l.Activo == true && l.IdLibro != libro.IdLibro))
                {
                    TempData["Error"] = "Ya existe otro libro con ese ISBN";
                    return RedirectToAction("Index");
                }

                // Actualizar campos (SIN STOCK)
                libroExistente.Titulo = libro.Titulo.Trim();
                libroExistente.ISBN = libro.ISBN.Trim();
                libroExistente.FechaPublicacion = libro.FechaPublicacion;
                libroExistente.ClasificacionEdad = libro.ClasificacionEdad ?? 0;
                libroExistente.Descripcion = string.IsNullOrWhiteSpace(libro.Descripcion) ? "Sin descripción" : libro.Descripcion.Trim();
                // IdStock ya no se actualiza

                // MANEJO DE IMAGEN UNIFICADO
                if (files != null && files.Length > 0)
                {
                    try
                    {
                        string webRootPath = _webHostEnvironment.WebRootPath;
                        string carpetaFisica = Path.Combine(webRootPath, "images", "libros");

                        if (!Directory.Exists(carpetaFisica))
                        {
                            Directory.CreateDirectory(carpetaFisica);
                        }

                        // Borrar imagen anterior si existe y no es la imagen por defecto
                        if (!string.IsNullOrEmpty(libroExistente.ImagenUrl) &&
                            libroExistente.ImagenUrl != "/images/no-image.png")
                        {
                            // Extraer solo el nombre del archivo de la URL
                            string oldFileName = Path.GetFileName(libroExistente.ImagenUrl);
                            string oldImagePath = Path.Combine(carpetaFisica, oldFileName);

                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                                Console.WriteLine($"Imagen anterior eliminada: {oldImagePath}");
                            }
                        }

                        // Guardar nueva imagen
                        string extension = Path.GetExtension(files.FileName);
                        string fileName = Guid.NewGuid().ToString() + extension;
                        string fullPath = Path.Combine(carpetaFisica, fileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            files.CopyTo(stream);
                        }

                        // Actualizar URL de la imagen
                        libroExistente.ImagenUrl = $"/images/libros/{fileName}";

                        Console.WriteLine($"Nueva imagen guardada: {fullPath}");
                        Console.WriteLine($"Nueva URL: {libroExistente.ImagenUrl}");
                    }
                    catch (Exception imgEx)
                    {
                        Console.WriteLine($"Error al actualizar imagen: {imgEx.Message}");
                        Console.WriteLine($"Stack trace: {imgEx.StackTrace}");
                        // No retornar error, continuar con la actualización sin imagen
                    }
                }

                _db.Libros.Update(libroExistente);
                _db.SaveChanges();

                Console.WriteLine($"Libro actualizado - ID: {libroExistente.IdLibro}, Título: {libroExistente.Titulo}");

                TempData["Success"] = "Libro actualizado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Edit: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                TempData["Error"] = "Error al actualizar el libro";
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Delete

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var libro = _db.Libros.Find(id);
                if (libro != null)
                {
                    // Verificar si tiene copias activas
                    var tieneCopiasActivas = _db.CopiasLibros
                        .Any(c => c.IdLibro == id && c.Activo == true);

                    if (tieneCopiasActivas)
                    {
                        TempData["Error"] = "No se puede eliminar el libro porque tiene copias activas. Elimina primero todas las copias.";
                        return RedirectToAction("Index");
                    }

                    // Soft delete
                    libro.Activo = false;
                    _db.Libros.Update(libro);
                    _db.SaveChanges();

                    TempData["Success"] = "Libro eliminado exitosamente";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar: {ex.Message}");
                TempData["Error"] = "Error al eliminar el libro";
                return RedirectToAction("Index");
            }
        }

        #endregion

        #region Nuevos métodos para gestión de copias

        // GET: Obtener copias de un libro específico
        [HttpGet]
        public IActionResult GetCopiasByLibro(int idLibro)
        {
            try
            {
                var libro = _db.Libros.Find(idLibro);
                if (libro == null)
                {
                    return NotFound("Libro no encontrado");
                }

                var copias = _db.CopiasLibros
                    .Include(c => c.EstadoCopiaLibro)
                    .Where(c => c.IdLibro == idLibro && c.Activo == true)
                    .ToList();

                ViewBag.Libro = libro;
                return PartialView("_CopiasLibroPartial", copias);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetCopiasByLibro: {ex.Message}");
                return BadRequest("Error al obtener las copias");
            }
        }

        // GET: Obtener stock calculado de un libro
        [HttpGet]
        public JsonResult GetStockLibro(int idLibro)
        {
            try
            {
                var stock = _db.CopiasLibros
                    .Count(c => c.IdLibro == idLibro && c.Activo == true);

                return Json(new { stock = stock });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetStockLibro: {ex.Message}");
                return Json(new { stock = 0 });
            }
        }

        #endregion
    }
}