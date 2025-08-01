using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Data;
using Models;

namespace Proyecto_Periodo_2.Controllers
{
    public class ClienteController : Controller
    {
        private readonly AppDbContext db;

        public ClienteController(AppDbContext _db)
        {
            db = _db;
        }

        // GET: UsuarioController - Listar todos los Clientes activos
        public ActionResult Index()
        {
            IEnumerable<Models.ViewModels.Cliente> clientes = db.Clientes
                .Where(c => c.Activo == true)
                .Select(c => new Models.ViewModels.Cliente {
                    IdCliente = c.IdCliente,
                    Cedula = c.Cedula,
                    Nombre = c.Nombre,
                    Apellido1 = c.Apellido1,
                    Apellido2 = c.Apellido2,
                    Correo = c.Correo,
                    Telefono = c.Telefono,
                    CantidadPrestamosDisponibles = c.CantidadPrestamosDisponibles,
                    Activo = c.Activo
                }).ToList();
            return View(clientes);
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

                var cliente = db.Clientes.Find(id); //El método Find() busca una entidad por su clave primaria(id).
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
                    var ClientesExistente = db.Clientes
                        .FirstOrDefault(u => u.Nombre == cliente.Nombre ||
                                           u.Cedula == cliente.Cedula);

                    if (ClientesExistente != null)
                    {
                        ModelState.AddModelError("", "Ya existe un Cliente con ese nombre de usuario o cédula.");
                        return View(cliente);
                    }

                    //si no
                    cliente.Activo = true;

                    db.Clientes.Add(cliente);
                    db.SaveChanges();


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

                var cliente = db.Clientes.Find(id);
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
                    var ClientesExistente = db.Clientes
                      .FirstOrDefault(u => u.Nombre == cliente.Nombre ||
                                          u.Cedula == cliente.Cedula);

                    if (ClientesExistente != null)
                    {
                        ModelState.AddModelError("", "Ya existe un Cliente con ese nombre de usuario o cédula.");
                        return View(cliente);
                    }
                    //actualizar
                    db.Clientes.Update(cliente);
                    db.SaveChanges();

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

                var cliente = db.Clientes.Find(id);
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
                var cliente = db.Clientes.Find(id);
                if (cliente != null)
                {
                    //cambiar Activo a false
                    cliente.Activo = false;
                    db.Clientes.Update(cliente);
                    db.SaveChanges();


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