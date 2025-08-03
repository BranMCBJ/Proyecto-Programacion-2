using Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.IO;
using System.Linq;
using Utilities;

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
                    .Include(l => l.Stock) // AGREGADO: Incluir la relación Stock
                    .Where(l => l.Activo == true).ToList();
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

        // POST: Libro/Create - RUTAS CORREGIDAS
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Libro libro, IFormFile files, int StockInicial = 0)
        {
            try
            {
                Console.WriteLine("=== MÉTODO POST CREATE CON STOCK ===");

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

                // PASO 1: Crear Stock con la cantidad inicial ingresada por el usuario
                var nuevoStock = new Stock
                {
                    Cantidad = StockInicial, // Cantidad ingresada por el usuario
                    Activo = true
                };

                _db.Stocks.Add(nuevoStock);
                _db.SaveChanges(); // Guardar para obtener el IdStock auto-incremental

                Console.WriteLine($"Stock creado - ID: {nuevoStock.IdStock}, Cantidad: {nuevoStock.Cantidad}");

                // PASO 2: Crear el libro vinculado al stock
                var nuevoLibro = new Libro
                {
                    Titulo = libro.Titulo?.Trim(),
                    ISBN = libro.ISBN?.Trim(),
                    FechaPublicacion = libro.FechaPublicacion,
                    ClasificacionEdad = libro.ClasificacionEdad ?? 0,
                    IdStock = nuevoStock.IdStock, // FK hacia el stock creado (auto-incremental)
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

                Console.WriteLine($"Libro creado - ID: {nuevoLibro.IdLibro}, Stock ID: {nuevoLibro.IdStock}");

                TempData["Success"] = $"¡Libro creado exitosamente con {StockInicial} copias en stock!";
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
                    return View(libro);
                }

                var libroExistente = _db.Libros.Find(libro.IdLibro);
                if (libroExistente == null)
                {
                    TempData["Error"] = "Libro no encontrado";
                    return RedirectToAction("Index");
                }

                // Actualizar campos
                libroExistente.Titulo = libro.Titulo;
                libroExistente.ISBN = libro.ISBN;
                libroExistente.FechaPublicacion = libro.FechaPublicacion;
                libroExistente.ClasificacionEdad = libro.ClasificacionEdad ?? 0;
                libroExistente.IdStock = libro.IdStock ?? 1;
                libroExistente.Descripcion = libro.Descripcion ?? "Sin descripción";

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
                    }
                }

                _db.Libros.Update(libroExistente);
                _db.SaveChanges();

                TempData["Success"] = "Libro actualizado exitosamente";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Edit: {ex.Message}");
                TempData["Error"] = "Error al actualizar el libro";
                return View(libro);
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
    }
}