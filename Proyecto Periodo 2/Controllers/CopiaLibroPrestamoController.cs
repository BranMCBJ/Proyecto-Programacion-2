using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Data;
using Models;

namespace Proyecto_Periodo_2.Controllers
{
    [Authorize] // Requiere autenticación para todo el controlador
    public class CopiaLibroPrestamoController : Controller
    {
        #region Propiedades y Constructor
        private readonly AppDbContext db;
        public CopiaLibroPrestamoController(AppDbContext db) 
        {
            this.db = db;
        }
        #endregion

        #region Accion de eliminar
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
        #endregion

        #region Accion de editar
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
        #endregion
    }
}
