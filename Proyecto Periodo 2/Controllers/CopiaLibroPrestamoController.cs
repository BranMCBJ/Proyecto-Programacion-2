using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Proyecto_Periodo_2.Datos;
using Proyecto_Periodo_2.Models;

namespace Proyecto_Periodo_2.Controllers
{
    public class CopiaLibroPrestamoController : Controller
    {
        private readonly AppDbContext db;
        public CopiaLibroPrestamoController(AppDbContext db)
        {
            this.db = db;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            CopiaLibroPrestamo? registro = db.CopiasLibrosPrestamos.FirstOrDefault(c => c.IdRelacion == id && c.Activo);
            if (registro != null)
            {
                registro.Activo = false;
                db.Update(registro);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return NotFound(); //Error
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CopiaLibroPrestamo registro)
        {
            bool disponible = db.CopiasLibrosPrestamos.Where(c => c.IdCopiaLibro == registro.IdCopiaLibro && c.Activo).IsNullOrEmpty();
            if (disponible)
            {
                db.Add(registro);
                db.SaveChanges();   
                return RedirectToAction("Index");
            }
            else 
            { 
                return NotFound(); //Error 
            }
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(CopiaLibroPrestamo clp)
        {
            if (ModelState.IsValid)
            {

            }
        }*/
    }
}
