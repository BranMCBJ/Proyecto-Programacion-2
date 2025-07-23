using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Periodo_2.Datos;
using Proyecto_Periodo_2.Models;
using System.Runtime.InteropServices;

namespace Proyecto_Periodo_2.Controllers
{
    public class CopiaLibroController : Controller
    {
        private readonly AppDbContext _db;
        public CopiaLibroController(AppDbContext db)
        {
            _db = db;
        }
        // GET: CopiaLibroController
        public ActionResult Index()
        {
            IEnumerable<CopiaLibro> listaLibros = _db.CopiasLibros.Where(c => c.Activo == true);
            return View(listaLibros);
        }

        // GET: CopiaLibroController/Details/5
        public ActionResult Details(int id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var libro = _db.CopiasLibros.Find(id);
            return View(libro);
        }

        // GET: CopiaLibroController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CopiaLibroController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CopiaLibro libro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.CopiasLibros.Add(libro);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                return View(libro);
            }
            catch (Exception
            )
            {
                throw;
            }
        }

        // GET: CopiaLibroController/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var libro = _db.CopiasLibros.Find(id);

                if (libro == null)
                {
                    return NotFound();
                }
                return View(libro);
            }
            catch (Exception)
            {

                throw;
            }
        }

        // POST: CopiaLibroController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CopiaLibro libro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.CopiasLibros.Update(libro);
                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
                return View(libro);
            }
            catch
            {
                return View();
            }
        }

        // GET: CopiaLibroController/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == 0 || id == null)
                {
                    return NotFound();
                }
                var libro = _db.CopiasLibros.Find(id);
                if (libro == null)
                {
                    return NotFound();
                }
                return View(libro);
            }
            catch (Exception)
            {

                throw;
            }
        }

        // POST: CopiaLibroController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }
                var libro = _db.CopiasLibros.Find(id);

                libro.Activo = false;

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