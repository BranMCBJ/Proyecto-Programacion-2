using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Data;
using Models;

namespace Proyecto_Periodo_2.Controllers
{
    public class StockController : Controller
    {
        private readonly AppDbContext _db;
        public StockController(AppDbContext db)
        {
            _db = db;
        }
        // GET: StockController
        public ActionResult Index()
        {
            return View();
        }

        // GET: StockController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: StockController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: StockController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CopiaLibro copia)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.CopiasLibros.Add(copia);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(Index));
                }
                return View(copia);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: StockController/Edit/5
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == 0 || id == null)
                {
                    return NotFound();
                }
                var stock = _db.Stocks.Find(id);
                if(stock == null || stock.Activo == false)
                {
                    return NotFound();
                }
                return View(stock);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: StockController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Stock stock)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _db.Stocks.Update(stock);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(LibroController.Index));
                }
                return View(stock);//ver si esta bien
            }
            catch (Exception)
            {
                throw;
            }
        }

        // GET: StockController/Delete/5
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == 0 || id == null)
                {
                    return NotFound();
                }
                var stock = _db.Stocks.Find(id);
                if (stock == null || stock.Activo == false)
                {
                    return NotFound();
                }
                return View(stock);
            }
            catch (Exception)
            {
                throw;
            }
        }

        // POST: StockController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Stock stock)
        {
            try
            {
                if(stock.Activo == true)
                {
                    stock.Activo = false;
                    _db.Stocks.Update(stock);
                    _db.SaveChanges();
                    return RedirectToAction(nameof(LibroController.Index));
                }
                return View(stock);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
