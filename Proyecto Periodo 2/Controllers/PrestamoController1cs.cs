using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proyecto_Periodo_2.Datos;
using Proyecto_Periodo_2.Models;

namespace Proyecto_Periodo_2.Controllers
{
    public class PrestamoController : Controller
    {
        // GET: PrestamosController
        private readonly AppDbContext _db;
        public PrestamoController(AppDbContext db)
        {
            _db = db;
        }

        public ActionResult Index()
        {
            IEnumerable<Prestamo> listaPrestamos = _db.Prestamo.Where(l => l.Activo == true);
            return View(listaPrestamos);
        }

        // GET: PrestamosController/Details/5
        public ActionResult Details(int id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var prestamo = _db.Prestamo.Find(id);
            return View(prestamo);
        }

        // GET: PrestamosController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PrestamosController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Prestamo prestamo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Prestamo.Add(prestamo);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                return View(prestamo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: PrestamosController/Edit/5
        public ActionResult Edit(int id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var prestamo = _db.Prestamo.Find(id);
            return View(prestamo);
        }

        // POST: PrestamosController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Prestamo prestamo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Prestamo.Update(prestamo);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                return View(prestamo);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: PrestamosController/Delete/5
        public ActionResult Delete(int id)
        {
            if (id == 0 || id == null)
            {
                return NotFound();
            }
            var prestamo = _db.Prestamo.Find(id);
            return View(prestamo);
        }

        // POST: PrestamosController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }
                var prestamo = _db.Prestamo.Find(id);

                if (prestamo == null)
                {
                    return NotFound();
                }

                prestamo.Activo = false;

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}