using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Proyecto_Periodo_2.Controllers
{
    public class CopiaLibroController : Controller
    {
        private readonly AppDbContext _db;

        public CopiaLibroController(AppDbContext db)
        {
            _db = db;
        }

        // GET: CopiaLibro
        public ActionResult Index()
        {
            try
            {
                var copias = _db.CopiasLibros
                    .Include(c => c.Libro)
                    .Include(c => c.EstadoCopiaLibro) // ✅ Corregido
                    .Where(c => c.Activo == true)
                    .OrderBy(c => c.Libro.Titulo)
                    .ThenBy(c => c.IdCopiaLibro)
                    .ToList();

                Console.WriteLine($"Copias encontradas: {copias.Count}");
                return View(copias);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Index: {ex.Message}");
                return View(new List<CopiaLibro>());
            }
        }

        // GET: CopiaLibro/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var copia = _db.CopiasLibros
                    .Include(c => c.Libro)
                    .Include(c => c.EstadoCopiaLibro) // ✅ Corregido
                    .FirstOrDefault(c => c.IdCopiaLibro == id);

                if (copia == null)
                {
                    return NotFound();
                }

                return View(copia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Details: {ex.Message}");
                return NotFound();
            }
        }

        // GET: CopiaLibro/Create
        public ActionResult Create(int? idLibro = null)
        {
            try
            {
                // Cargar libros activos para el dropdown
                ViewBag.Libros = new SelectList(
                    _db.Libros.Where(l => l.Activo == true).ToList(),
                    "IdLibro",
                    "Titulo",
                    idLibro
                );

                // ✅ Cambiado a usar "Nombre" en lugar de "NombreEstado"
                ViewBag.Estados = new SelectList(
                    _db.EstadoCopiaLibro.Where(e => e.Activo == true).ToList(),
                    "IdEstadoCopialibro", // ✅ Usando el nombre exacto de tu modelo
                    "Nombre"
                );

                // Si viene un ID de libro específico, pre-seleccionarlo
                if (idLibro.HasValue)
                {
                    var libro = _db.Libros.Find(idLibro.Value);
                    if (libro != null)
                    {
                        ViewBag.LibroSeleccionado = libro;
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Create GET: {ex.Message}");
                return RedirectToAction("Index");
            }
        }

        // POST: CopiaLibro/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CopiaLibro copiaLibro)
        {
            try
            {
                // ✅ Validaciones ajustadas para int? nullable
                if (copiaLibro.IdLibro == null || copiaLibro.IdLibro == 0)
                {
                    TempData["Error"] = "Debe seleccionar un libro";
                    return RedirectToAction("Create", new { idLibro = copiaLibro.IdLibro });
                }

                if (copiaLibro.IdEstadoCopiaLibro == null || copiaLibro.IdEstadoCopiaLibro == 0)
                {
                    TempData["Error"] = "Debe seleccionar un estado";
                    return RedirectToAction("Create", new { idLibro = copiaLibro.IdLibro });
                }

                // Verificar que el libro existe y está activo
                var libro = _db.Libros.Find(copiaLibro.IdLibro);
                if (libro == null || libro.Activo == false)
                {
                    TempData["Error"] = "El libro seleccionado no existe o no está activo";
                    return RedirectToAction("Index");
                }

                // ✅ Usando el nombre correcto de la clave primaria
                var estado = _db.EstadoCopiaLibro.Find(copiaLibro.IdEstadoCopiaLibro);
                if (estado == null || estado.Activo == false)
                {
                    TempData["Error"] = "El estado seleccionado no existe o no está activo";
                    return RedirectToAction("Create", new { idLibro = copiaLibro.IdLibro });
                }

                // Crear la nueva copia
                var nuevaCopia = new CopiaLibro
                {
                    IdLibro = copiaLibro.IdLibro,
                    IdEstadoCopiaLibro = copiaLibro.IdEstadoCopiaLibro,
                    Activo = true
                };

                _db.CopiasLibros.Add(nuevaCopia);
                _db.SaveChanges();

                Console.WriteLine($"Copia creada - ID: {nuevaCopia.IdCopiaLibro}, Libro: {libro.Titulo}");

                TempData["Success"] = $"¡Copia del libro '{libro.Titulo}' creada exitosamente!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Create POST: {ex.Message}");
                TempData["Error"] = "Error al crear la copia del libro";
                return RedirectToAction("Create", new { idLibro = copiaLibro.IdLibro });
            }
        }

        // GET: CopiaLibro/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var copia = _db.CopiasLibros
                    .Include(c => c.Libro)
                    .Include(c => c.EstadoCopiaLibro) // ✅ Corregido
                    .FirstOrDefault(c => c.IdCopiaLibro == id);

                if (copia == null)
                {
                    return NotFound();
                }

                // ✅ Cambiado para usar tu modelo actual
                ViewBag.Estados = new SelectList(
                    _db.EstadoCopiaLibro.Where(e => e.Activo == true).ToList(),
                    "IdEstadoCopialibro", // ✅ Usando el nombre exacto de tu modelo
                    "Nombre",
                    copia.IdEstadoCopiaLibro
                );

                return View(copia);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Edit GET: {ex.Message}");
                return NotFound();
            }
        }

        // POST: CopiaLibro/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CopiaLibro copiaLibro)
        {
            try
            {
                if (copiaLibro.IdCopiaLibro == null || copiaLibro.IdCopiaLibro == 0)
                {
                    TempData["Error"] = "ID de copia inválido";
                    return RedirectToAction("Index");
                }

                var copiaExistente = _db.CopiasLibros
                    .Include(c => c.Libro)
                    .FirstOrDefault(c => c.IdCopiaLibro == copiaLibro.IdCopiaLibro);

                if (copiaExistente == null)
                {
                    TempData["Error"] = "Copia no encontrada";
                    return RedirectToAction("Index");
                }

                // Validar el nuevo estado
                if (copiaLibro.IdEstadoCopiaLibro == null || copiaLibro.IdEstadoCopiaLibro == 0)
                {
                    TempData["Error"] = "Debe seleccionar un estado válido";
                    return RedirectToAction("Edit", new { id = copiaLibro.IdCopiaLibro });
                }

                var estado = _db.EstadoCopiaLibro.Find(copiaLibro.IdEstadoCopiaLibro);
                if (estado == null || estado.Activo == false)
                {
                    TempData["Error"] = "El estado seleccionado no es válido";
                    return RedirectToAction("Edit", new { id = copiaLibro.IdCopiaLibro });
                }

                // Actualizar solo el estado (IdLibro no se puede cambiar)
                copiaExistente.IdEstadoCopiaLibro = copiaLibro.IdEstadoCopiaLibro;

                _db.CopiasLibros.Update(copiaExistente);
                _db.SaveChanges();

                Console.WriteLine($"Copia actualizada - ID: {copiaExistente.IdCopiaLibro}");

                TempData["Success"] = $"¡Copia del libro '{copiaExistente.Libro?.Titulo}' actualizada exitosamente!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en Edit POST: {ex.Message}");
                TempData["Error"] = "Error al actualizar la copia";
                return RedirectToAction("Edit", new { id = copiaLibro.IdCopiaLibro });
            }
        }

        // POST: CopiaLibro/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var copia = _db.CopiasLibros
                    .Include(c => c.Libro)
                    .FirstOrDefault(c => c.IdCopiaLibro == id);

                if (copia != null)
                {
                    // Verificar si la copia está en préstamo (si tienes esa funcionalidad)
                    // var enPrestamo = _db.Prestamos.Any(p => p.IdCopiaLibro == id && p.Activo == true);
                    // if (enPrestamo)
                    // {
                    //     TempData["Error"] = "No se puede eliminar una copia que está en préstamo";
                    //     return RedirectToAction("Index");
                    // }

                    // Soft delete
                    copia.Activo = false;
                    _db.CopiasLibros.Update(copia);
                    _db.SaveChanges();

                    Console.WriteLine($"Copia eliminada - ID: {copia.IdCopiaLibro}");

                    TempData["Success"] = $"Copia del libro '{copia.Libro?.Titulo}' eliminada exitosamente";
                }
                else
                {
                    TempData["Error"] = "Copia no encontrada";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar copia: {ex.Message}");
                TempData["Error"] = "Error al eliminar la copia";
                return RedirectToAction("Index");
            }
        }

        #region Métodos auxiliares para gestión específica por libro

        // GET: Obtener copias de un libro específico (para modal)
        [HttpGet]
        public ActionResult CopiasPorLibro(int idLibro)
        {
            try
            {
                var libro = _db.Libros.Find(idLibro);
                if (libro == null)
                {
                    return NotFound("Libro no encontrado");
                }

                var copias = _db.CopiasLibros
                    .Include(c => c.EstadoCopiaLibro) // ✅ Corregido
                    .Where(c => c.IdLibro == idLibro && c.Activo == true)
                    .OrderBy(c => c.IdCopiaLibro)
                    .ToList();

                ViewBag.Libro = libro;
                ViewBag.TotalCopias = copias.Count;

                return View(copias);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CopiasPorLibro: {ex.Message}");
                return RedirectToAction("Index", "Libro");
            }
        }

        // POST: Crear múltiples copias de un libro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearMultiplesCopias(int idLibro, int cantidad, int idEstado)
        {
            try
            {
                if (cantidad <= 0 || cantidad > 50) // Límite de seguridad
                {
                    TempData["Error"] = "La cantidad debe ser entre 1 y 50 copias";
                    return RedirectToAction("CopiasPorLibro", new { idLibro = idLibro });
                }

                var libro = _db.Libros.Find(idLibro);
                if (libro == null || libro.Activo == false)
                {
                    TempData["Error"] = "Libro no encontrado o inactivo";
                    return RedirectToAction("Index", "Libro");
                }

                var estado = _db.EstadoCopiaLibro.Find(idEstado);
                if (estado == null || estado.Activo == false)
                {
                    TempData["Error"] = "Estado no válido";
                    return RedirectToAction("CopiasPorLibro", new { idLibro = idLibro });
                }

                // Crear las copias
                var copiasCreadas = 0;
                for (int i = 0; i < cantidad; i++)
                {
                    var nuevaCopia = new CopiaLibro
                    {
                        IdLibro = idLibro,
                        IdEstadoCopiaLibro = idEstado,
                        Activo = true
                    };

                    _db.CopiasLibros.Add(nuevaCopia);
                    copiasCreadas++;
                }

                _db.SaveChanges();

                Console.WriteLine($"Creadas {copiasCreadas} copias del libro ID: {idLibro}");

                TempData["Success"] = $"¡{copiasCreadas} copias del libro '{libro.Titulo}' creadas exitosamente!";
                return RedirectToAction("CopiasPorLibro", new { idLibro = idLibro });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en CrearMultiplesCopias: {ex.Message}");
                TempData["Error"] = "Error al crear las copias";
                return RedirectToAction("CopiasPorLibro", new { idLibro = idLibro });
            }
        }

        // AJAX: Obtener estadísticas de copias por libro
        [HttpGet]
        public JsonResult GetEstadisticasCopias(int idLibro)
        {
            try
            {
                var copias = _db.CopiasLibros
                    .Include(c => c.EstadoCopiaLibro) // ✅ Corregido
                    .Where(c => c.IdLibro == idLibro && c.Activo == true)
                    .ToList();

                var estadisticas = copias
                    .GroupBy(c => c.EstadoCopiaLibro?.Nombre ?? "Sin Estado") // ✅ Usando "Nombre" de tu modelo
                    .Select(g => new
                    {
                        estado = g.Key,
                        cantidad = g.Count()
                    })
                    .ToList();

                return Json(new
                {
                    total = copias.Count,
                    estadisticas = estadisticas
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en GetEstadisticasCopias: {ex.Message}");
                return Json(new { total = 0, estadisticas = new List<object>() });
            }
        }

        #endregion
    }
}