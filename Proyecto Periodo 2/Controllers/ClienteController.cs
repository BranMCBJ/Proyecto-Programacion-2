using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Proyecto_Periodo_2.Datos;
using Proyecto_Periodo_2.Models;

namespace Proyecto_Periodo_2.Controllers
{
    public class ClienteController : Controller
    {
        private readonly AppDbContext _db;

        public ClienteController(AppDbContext db)
        {
            _db = db;
        }

        // GET: UsuarioController - Listar todos los Clientes activos
        public ActionResult Index()
        {
            try
            {
                IEnumerable<Cliente> listaClientes = _db.Clientes.Where(c => c.Activo == true);
                return View(listaClientes);
            }
            catch (Exception)
            {
                // En caso de error, retorna una lista vacía
                return View(new List<Cliente>());
            }
        }

        // GET: 
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var cliente = _db.Clientes.Find(id); //El método Find() busca una entidad por su clave primaria(id).
                if (cliente == null || cliente.Activo != true)
                {
                    return NotFound();
                }

                return View(cliente);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: 
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuarioController/Create - Crear nuevo usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cliente cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar si ya existe un Clientes con el mismo nombre de usuario o cédula
                    var ClientesExistente = _db.Clientes
                        .FirstOrDefault(u => u.Nombre == cliente.Nombre ||
                                           u.Cedula == cliente.Cedula);

                    if (ClientesExistente != null)
                    {
                        ModelState.AddModelError("", "Ya existe un Cliente con ese nombre de usuario o cédula.");
                        return View(cliente);
                    }

                    //si no
                    cliente.Activo = true;

                    _db.Clientes.Add(cliente);
                    _db.SaveChanges();


                    return RedirectToAction(nameof(Index));
                }
                return View(cliente);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al crear un Cliente");
                return View(cliente);
            }
        }

        // GET: 
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var cliente = _db.Clientes.Find(id);
                if (cliente == null)
                {
                    return NotFound();
                }

                return View(cliente);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cliente cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var ClientesExistente = _db.Clientes
                      .FirstOrDefault(u => u.Nombre == cliente.Nombre ||
                                          u.Cedula == cliente.Cedula);

                    if (ClientesExistente != null)
                    {
                        ModelState.AddModelError("", "Ya existe un Cliente con ese nombre de usuario o cédula.");
                        return View(cliente);
                    }
                    //actualizar
                    _db.Clientes.Update(cliente);
                    _db.SaveChanges();

                    //TempData["Mensaje"] = "Usuario actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                return View(cliente);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al actualizar el Cliente");
                return View(cliente);
            }
        }

        // GET: eliminación
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var cliente = _db.Clientes.Find(id);
                if (cliente == null)
                {
                    return NotFound();
                }

                return View(cliente);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST:  Eliminar  
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var cliente = _db.Clientes.Find(id);
                if (cliente != null)
                {
                    //cambiar Activo a false
                    cliente.Activo = false;
                    _db.Clientes.Update(cliente);
                    _db.SaveChanges();


                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al Eliminar el Cliente");
                return RedirectToAction(nameof(Index));
            }
        }

    }
}