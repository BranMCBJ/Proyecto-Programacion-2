using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proyecto_Periodo_2.Datos;
using Proyecto_Periodo_2.Models;

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
            IEnumerable<Libro> listaLibros = _db.Libros;
            return View(listaLibros);
        }

        // GET: CopiaLibroController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CopiaLibroController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CopiaLibroController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Libro libro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Libros.Add(libro);
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
                if(id == null || id == 0)
                {
                    return NotFound();
                }

                var libro = _db.Libros.Find(id);

                if(libro == null)
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
        public ActionResult Edit(Libro libro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Libros.Update(libro);
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
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CopiaLibroController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
