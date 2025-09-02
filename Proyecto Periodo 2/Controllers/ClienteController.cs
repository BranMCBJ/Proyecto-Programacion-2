using Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Utilities;

namespace Proyecto_Periodo_2.Controllers
{
    /// <summary>
    /// Controlador para manejar operaciones CRUD de clientes
    /// </summary>
    [Authorize] // Requiere autenticación para todo el controlador
    public class ClienteController : Controller
    {
        private readonly AppDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;

        /// <summary>
        /// Constructor del controlador Cliente
        /// </summary>
        public ClienteController(AppDbContext _db, IWebHostEnvironment _webHostEnvironment)
        {
            db = _db;
            webHostEnvironment = _webHostEnvironment;
        }

        /// <summary>
        /// Muestra la lista de clientes activos
        /// </summary>
        public ActionResult Index()
        {
            // Obtener todos los clientes activos con sus datos principales
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
                    Activo = c.Activo,
                    URLImagen = c.URLImagen
                }).ToList();

            return View(clientes);
        }

        /// <summary>
        /// Crea un nuevo cliente en el sistema
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cliente cliente, IFormFile ImageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar si ya existe un cliente con el mismo nombre o cédula
                    var clienteExistente = db.Clientes
                        .FirstOrDefault(u => (u.Nombre == cliente.Nombre ||
                                             u.Cedula == cliente.Cedula) && u.Activo == true);

                    if (clienteExistente != null)
                    {
                        TempData["Error"] = "Ya existe un Cliente con ese nombre o cédula.";
                        return RedirectToAction(nameof(Index));
                    }

                    cliente.Activo = true;

                    // Procesamiento de imagen del cliente
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        // Validar tipo de archivo permitido
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                        var extension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
                        
                        if (!allowedExtensions.Contains(extension))
                        {
                            TempData["Error"] = "Solo se permiten archivos de imagen (jpg, jpeg, png, gif, bmp).";
                            return RedirectToAction(nameof(Index));
                        }

                        // Validar tamaño máximo del archivo (5MB)
                        if (ImageFile.Length > 5 * 1024 * 1024)
                        {
                            TempData["Error"] = "El archivo de imagen no puede superar los 5MB.";
                            return RedirectToAction(nameof(Index));
                        }

                        // Crear directorio si no existe
                        string webRootPath = webHostEnvironment.WebRootPath;
                        string upload = Path.Combine(webRootPath, "Images", "Cliente");
                        
                        if (!Directory.Exists(upload))
                        {
                            Directory.CreateDirectory(upload);
                        }
                        
                        // Generar nombre único para el archivo
                        string fileName = Guid.NewGuid().ToString() + extension;

                        // Guardar archivo en el servidor
                        using (var fileStream = new FileStream(Path.Combine(upload, fileName), FileMode.Create))
                        {
                            ImageFile.CopyTo(fileStream);
                        }

                        cliente.URLImagen = fileName;
                    }

                    db.Clientes.Add(cliente);
                    db.SaveChanges();

                    TempData["Exito"] = "Cliente creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                
                // Si el modelo no es válido, mostrar errores específicos
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Datos inválidos: " + string.Join(", ", errores);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear el Cliente: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Cliente cliente, IFormFile ImageFile)
        {
            try
            {
                // Debug: Log valores recibidos
                System.Diagnostics.Debug.WriteLine($"Edit llamado - ID: {cliente.IdCliente}, Nombre: {cliente.Nombre}, Cedula: {cliente.Cedula}");
                
                var clienteDb = db.Clientes.Find(cliente.IdCliente);
                if (clienteDb == null)
                {
                    TempData["Error"] = "No se encontró el cliente especificado.";
                    return RedirectToAction(nameof(Index));
                }

                var cedulaExistente = db.Clientes
                    .Any(c => c.Cedula == cliente.Cedula && c.IdCliente != cliente.IdCliente);
                if (cedulaExistente)
                {
                    TempData["Error"] = "Ya existe un cliente con la misma cédula.";
                    return RedirectToAction(nameof(Index));
                }

                // Actualizar datos básicos
                clienteDb.Nombre = cliente.Nombre;
                clienteDb.Apellido1 = cliente.Apellido1;
                clienteDb.Apellido2 = cliente.Apellido2;
                clienteDb.Cedula = cliente.Cedula;
                clienteDb.Correo = cliente.Correo;
                clienteDb.Telefono = cliente.Telefono;
                clienteDb.CantidadPrestamosDisponibles = cliente.CantidadPrestamosDisponibles;

                // Manejo de la nueva imagen si se proporcionó
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    // Validar tipo de archivo
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    var extension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["Error"] = "Solo se permiten archivos de imagen (jpg, jpeg, png, gif, bmp).";
                        return RedirectToAction(nameof(Index));
                    }

                    // Validar tamaño (máximo 5MB)
                    if (ImageFile.Length > 5 * 1024 * 1024)
                    {
                        TempData["Error"] = "El archivo de imagen no puede superar los 5MB.";
                        return RedirectToAction(nameof(Index));
                    }

                    string webRootPath = webHostEnvironment.WebRootPath;
                    string upload = Path.Combine(webRootPath, "Images", "Cliente");
                    
                    if (!Directory.Exists(upload))
                    {
                        Directory.CreateDirectory(upload);
                    }

                    // Eliminar la imagen anterior si existe
                    if (!string.IsNullOrEmpty(clienteDb.URLImagen))
                    {
                        string oldImagePath = Path.Combine(upload, clienteDb.URLImagen);
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    
                    string fileName = Guid.NewGuid().ToString() + extension;

                    using (var fileStream = new FileStream(Path.Combine(upload, fileName), FileMode.Create))
                    {
                        ImageFile.CopyTo(fileStream);
                    }

                    clienteDb.URLImagen = fileName;
                }

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
                    TempData["Exito"] = "Cliente eliminado correctamente.";
                }
                else
                {
                    TempData["Error"] = "No se encontró el cliente especificado.";
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el cliente: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Suspender(Cliente cliente)
        {
            var clienteBD = db.Clientes.Find(cliente.IdCliente);
            if (clienteBD == null)
            {
                return NotFound();
            }

            // Solo modificar lo necesario
            clienteBD.CantidadPrestamosDisponibles = 0;

            db.Clientes.Update(clienteBD);
            db.SaveChanges();

            return RedirectToAction("Index", "Prestamo");
        }
    }
}
