using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                .Select(c => new Models.ViewModels.Cliente
                {
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

        // GET: Detalles de cliente
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null || id == 0)
                    return NotFound();

                var cliente = db.Clientes.Find(id);
                if (cliente == null || cliente.Activo != true)
                    return NotFound();

                return View(cliente);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: Crear cliente
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuarioController/Create - Crear nuevo cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cliente cliente)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar si ya existe un Cliente con el mismo nombre o cédula
                    var clienteExistente = db.Clientes
                        .FirstOrDefault(u => u.Nombre == cliente.Nombre ||
                                             u.Cedula == cliente.Cedula);

                    if (clienteExistente != null)
                    {
                        ModelState.AddModelError("", "Ya existe un Cliente con ese nombre o cédula.");
                        return View(cliente);
                    }

                    cliente.Activo = true;
                    db.Clientes.Add(cliente);
                    db.SaveChanges();

                    return RedirectToAction(nameof(Index));
                }
                return View(cliente);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al crear el Cliente.");
                return View(cliente);
            }
        }

        // POST: Editar cliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cliente cliente)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Los datos enviados no son válidos.";
                    return RedirectToAction(nameof(Index));
                }

                var clienteDb = db.Clientes.Find(cliente.IdCliente);
                if (clienteDb == null)
                {
                    TempData["Error"] = "No se encontró el cliente especificado.";
                    return RedirectToAction(nameof(Index));
                }

                // Validar cédula única
                var cedulaExistente = db.Clientes
                    .Any(c => c.Cedula == cliente.Cedula && c.IdCliente != cliente.IdCliente);
                if (cedulaExistente)
                {
                    TempData["Error"] = "Ya existe un cliente con la misma cédula.";
                    return RedirectToAction(nameof(Index));
                }

                // Actualizar campos
                clienteDb.Nombre = cliente.Nombre;
                clienteDb.Apellido1 = cliente.Apellido1;
                clienteDb.Apellido2 = cliente.Apellido2;
                clienteDb.Cedula = cliente.Cedula;
                clienteDb.Correo = cliente.Correo;
                clienteDb.Telefono = cliente.Telefono;
                clienteDb.CantidadPrestamosDisponibles = cliente.CantidadPrestamosDisponibles;

                db.Clientes.Update(clienteDb);
                db.SaveChanges();

                TempData["Exito"] = "Cliente actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                TempData["Error"] = "Error en la base de datos al editar el cliente: " + dbEx.Message;
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Ocurrió un error inesperado: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Eliminación
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null || id == 0)
                    return NotFound();

                var cliente = db.Clientes.Find(id);
                if (cliente == null)
                    return NotFound();

                return View(cliente);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: Eliminar (marcar como inactivo)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var cliente = db.Clientes.Find(id);
                if (cliente != null)
                {
                    cliente.Activo = false;
                    db.Clientes.Update(cliente);
                    db.SaveChanges();
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al eliminar el Cliente.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
