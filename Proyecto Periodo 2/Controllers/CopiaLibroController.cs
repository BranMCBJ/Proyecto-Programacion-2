using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        [HttpGet]
        public ActionResult AgregarCopia()
        {
            return PartialView("_PartialCrearCopiaLibro");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CrearCopia(CopiaLibro copia)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.CopiasLibros.Add(copia);
                    _db.SaveChanges();
                    TempData["Exito"] = "Se agrego una copia del libro";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = "Error al agregra la copia del libro";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var error = ex.ToString();
                TempData["Error"] = error;
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult e()
        {
            return View();
        }
    }
}