using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models.ViewModels;

namespace Proyecto_Periodo_2.Controllers
{
    [Authorize] // Requiere autenticación para todo el controlador
    public class CopiaLibroController : Controller
    {
        private readonly AppDbContext _db;

        public CopiaLibroController(AppDbContext db)
        {
            _db = db;
        }

        // GET: CopiaLibro
        public ActionResult Index(int? id)
        {
            var copias = _db.CopiasLibros
                .Include(c => c.Libro)
                .Include(c => c.EstadoCopiaLibro)
                .Where(c => c.Libro.IdLibro == id && c.Activo == true)
                .ToList();
            if (copias == null || !copias.Any())
            {
                TempData["Error"] = "No se encontraron copias de libro.";
                return RedirectToAction("Index", "Libro");
            }
            return PartialView("IndexCopiaLibro", copias);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AgregarCopiaLibro(CopiaLibro copiaLibro)
        {
            if (ModelState.IsValid)
            {
                _db.CopiasLibros.Add(copiaLibro);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Modelo de copia del libro invalido";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarCopiaLibro(int id)
        {
            var copia = _db.CopiasLibros.Find(id);

            if (copia == null)
            {
                TempData["Error"] = "Copia de libro no encontrada"
                return RedirectToAction(nameof(Index));
            }
            copia.Activo = false;
            _db.CopiasLibros.Update(copia);
            _db.SaveChanges();
            TempData["Exito"] = "Copia de libro Agregada con exito";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarCopiaLibro(CopiaLibro copiaLibro)
        {
            if (ModelState.IsValid)
            {
                _db.CopiasLibros.Update(copiaLibro);
                _db.SaveChanges();
                TempData["Exito"] = "Copia del libro se actulizo con exito";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Error al actualizar la copia del libro";
            return RedirectToAction(nameof(Index));
        }

        public ActionResult AgregarCopia()
        {
            return PartialView("_PartialCrearCopiaLibro");
        }
    }
}