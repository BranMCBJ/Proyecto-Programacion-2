using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;

namespace Proyecto_Periodo_2.Controllers
{
    [Authorize] // Requiere autenticación para todo el controlador
    public class EstadoPrestamoController : Controller
    {
        private readonly AppDbContext _db;

        public EstadoPrestamoController(AppDbContext db)
        {
            _db = db;
        }

        // GET: EstadoPrestamoController - Listar todos los estados activos
        public ActionResult Index()
        {
            try
            {
                IEnumerable<EstadoPrestamo> listaEstados = _db.EstadoPrestamo.Where(e => e._Activo == true);
                return View(listaEstados);
            }
            catch (Exception)
            {
                // En caso de error, retorna una lista vacía
                return View(new List<EstadoPrestamo>());
            }
        }

        // GET: EstadoPrestamoController/Details/5 - Ver detalles de un estado
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var estado = _db.EstadoPrestamo.Find(id);
                if (estado == null || estado._Activo != true)
                {
                    return NotFound();
                }

                return View(estado);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: EstadoPrestamoController/Create - Mostrar formulario de creación
        public ActionResult Create()
        {
            return View();
        }

        // POST: EstadoPrestamoController/Create - Crear nuevo estado de préstamo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EstadoPrestamo estadoPrestamo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar si ya existe un estado con el mismo nombre
                    var estadoExistente = _db.EstadoPrestamo
                        .FirstOrDefault(e => e._Nombre == estadoPrestamo._Nombre && e._Activo == true);

                    if (estadoExistente != null)
                    {
                        ModelState.AddModelError("", "Ya existe un estado de préstamo con ese nombre.");
                        return View(estadoPrestamo);
                    }

                    // Establecer estado como activo por defecto
                    estadoPrestamo._Activo = true;

                    _db.EstadoPrestamo.Add(estadoPrestamo);
                    _db.SaveChanges();

                    TempData["Mensaje"] = "Estado de préstamo creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                return View(estadoPrestamo);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al crear el estado de préstamo");
                return View(estadoPrestamo);
            }
        }

        // GET: EstadoPrestamoController/Edit/5 - Mostrar formulario de edición
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var estado = _db.EstadoPrestamo.Find(id);
                if (estado == null)
                {
                    return NotFound();
                }

                return View(estado);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: EstadoPrestamoController/Edit/5 - Actualizar estado de préstamo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EstadoPrestamo estadoPrestamo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar si ya existe otro estado con el mismo nombre
                    var estadoExistente = _db.EstadoPrestamo
                        .FirstOrDefault(e => e._Nombre == estadoPrestamo._Nombre &&
                                           e._IdEstado != estadoPrestamo._IdEstado &&
                                           e._Activo == true);

                    if (estadoExistente != null)
                    {
                        ModelState.AddModelError("", "Ya existe otro estado de préstamo con ese nombre.");
                        return View(estadoPrestamo);
                    }

                    _db.EstadoPrestamo.Update(estadoPrestamo);
                    _db.SaveChanges();

                    TempData["Mensaje"] = "Estado de préstamo actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                return View(estadoPrestamo);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al actualizar el estado de préstamo");
                return View(estadoPrestamo);
            }
        }

        // GET: EstadoPrestamoController/Delete/5 - Confirmar eliminación
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var estado = _db.EstadoPrestamo.Find(id);
                if (estado == null)
                {
                    return NotFound();
                }

                return View(estado);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: EstadoPrestamoController/Delete/5 - Eliminar estado (eliminación lógica)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var estado = _db.EstadoPrestamo.Find(id);
                if (estado != null)
                {
                    estado._Activo = false;
                    _db.EstadoPrestamo.Update(estado);
                    _db.SaveChanges();

                    TempData["Mensaje"] = "Estado de préstamo eliminado exitosamente";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al eliminar el estado de préstamo";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}