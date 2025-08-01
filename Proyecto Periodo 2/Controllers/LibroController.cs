using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Data;
using Models;

namespace Proyecto_Periodo_2.Controllers
{
    public class LibroController : Controller
    {
        private readonly AppDbContext _db;
        public LibroController(AppDbContext db)
        {
            _db = db;
        }
        public ActionResult Index()
        {
            try
            {
                IEnumerable<Libro> listaLibros = _db.Libros.Where(u => u.Activo == true);
                return View(listaLibros);
            }
            catch (Exception)
            {
                // En caso de error, retorna una lista vacía
                return View(new List<Libro>());
            }
        }
        // GET:  Ver detalles 
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var libro = _db.Libros.Find(id); //El método Find() busca una entidad por su clave primaria(id).
                if (libro == null || libro.Activo != true)
                {
                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


        #region create 


        // GET:Create
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Libro Libro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar si ya existe un Libro con el mismo nombre 
                    var LibroExistente = _db.Libros
                        .FirstOrDefault(l => l.Titulo
                                             == Libro.Titulo || l.ISBN == Libro.ISBN);

                    if (LibroExistente != null)
                    {
                        ModelState.AddModelError("", "Ya existe un Libro con ese Titulo o ISBN.");
                        return View(Libro);
                    }

                    //si no
                    Libro.Activo = true;

                    _db.Libros.Add(Libro);
                    _db.SaveChanges();


                    return RedirectToAction(nameof(Index));
                }
                return View(Libro);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al crear el nuevo libro");
                return View(Libro);
            }
        }

        #endregion

        #region Edit


        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var libro = _db.Libros.Find(id);
                if (libro == null)
                {
                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Libro libro)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar si ya existe otro libro con el mismo titulo o ISBN 
                    var LibroExistente = _db.Libros
                        .FirstOrDefault(l => l.Titulo
                                             == libro.Titulo || l.ISBN == libro.ISBN);

                    if (LibroExistente != null)
                    {
                        ModelState.AddModelError("", "Ya existe un Libro con ese Titulo o ISBN.");
                        return View(libro);
                    }



                    //actualizar
                    _db.Libros.Update(libro);
                    _db.SaveChanges();


                    return RedirectToAction(nameof(Index));
                }
                //recargar todo
                return View(libro);

            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al actualizar el libro");
                return View(libro);
            }
        }



        #endregion

        #region Delete


        // GET: 
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var libro = _db.Libros.Find(id);
                if (libro == null)
                {
                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST:  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var libro = _db.Libros.Find(id);
                if (libro != null)
                {
                    //cambiar Activo a false
                    libro.Activo = false;
                    _db.Libros.Update(libro);
                    _db.SaveChanges();


                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {

                return RedirectToAction(nameof(Index));
            }
        }


        #endregion


    }

}
