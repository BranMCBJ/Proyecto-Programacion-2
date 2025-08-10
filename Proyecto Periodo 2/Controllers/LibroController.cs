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
                var libros = _db.Libros.Where(c => c.Activo == true).ToList();
                return View(libros);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View(new List<LibroVM>());
            }
        }
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
                //saca el libro por el id
                var libroVM = new LibroVM()
                {
                    Libro = _db.Libros.FirstOrDefault(s =>
                s.IdLibro == id &&
                s.Activo == true),

                    Imagen = null
                };
                return PartialView("_PartialVerMasLibro", libroVM);
            }
            catch (Exception ex)
            {
                // Manejo de errores
                ViewBag.Error = ex.Message;
                TempData["Error"] = "Error al cargar el libro.";
                return View(nameof(Index));
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearLibro(LibroVM libroVM)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var imagen = libroVM.Imagen;
                    var libro = libroVM.Libro;

                    if (imagen != null && imagen.Length > 0)
                    {
                        // Guardar la imagen en el servidor
                        var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "imagenes");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                        var filePath = Path.Combine(uploadsFolder, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            imagen.CopyTo(stream);
                        }
                        libroVM.Libro.ImagenUrl = "/Libros/images/" + fileName;
                    }
                    libro.Activo = true; // Asegurarse de que el libro esté activo
                    _db.Libros.Add(libro);
                    _db.SaveChanges();
                    TempData["Success"] = "Libro creado exitosamente.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al crear el libro: " + ex.Message;
                    return View(nameof(Index));
                }
            }
            return PartialView("_PartialCrearLibro", libroVM);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpDelete(LibroVM libroVM, string accion)
        {
            if (accion == "actualizar")
            {
                if (!ModelState.IsValid)
                {
                    return PartialView("_PartialVerMasLibro", libroVM);
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
                    // Guarda en wwwroot/Libros/images para mantener consistencia con las URLs usadas
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Libros", "images");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imagen.FileName);
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imagen.CopyTo(stream);
                    }

                    // Asigna la URL pública coherente con la carpeta anterior
                    libroEnDb.ImagenUrl = $"/Libros/images/{fileName}";
                }

                _db.SaveChanges();
                TempData["Success"] = "Libro actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            if (accion == "eliminar")
            {
                var libroEnDb = _db.Libros.FirstOrDefault(x => x.IdLibro == libroVM.Libro.IdLibro);
                if (libroEnDb == null)
                {
                    TempData["Error"] = "Libro no encontrado.";
                    return RedirectToAction(nameof(Index));
                }

                // Soft delete
                libroEnDb.Activo = false;
                _db.SaveChanges();
                TempData["Success"] = "Libro eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Acción no válida.";
            return RedirectToAction(nameof(Index));
        }

    }
}