using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Data;
using System.Text.Json;
using System.Runtime.ExceptionServices;

namespace Proyecto_Periodo_2.Controllers
{
    /// <summary>
    /// Controlador para manejar operaciones de préstamos de libros
    /// </summary>
    [Authorize] // Requiere autenticación para todo el controlador
    public class PrestamoController : Controller
    {
        private readonly AppDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private string ruta;
        
        // Variable para mantener los datos del préstamo en proceso
        private Models.ViewModels.PrestamoCreate nuevoPrestamo = new Models.ViewModels.PrestamoCreate()
        {
            copiasLibro = new List<CopiaLibro>()
        };

        /// <summary>
        /// Constructor del controlador de préstamos
        /// </summary>
        public PrestamoController(AppDbContext _db, IWebHostEnvironment _webHostEnvironment)
        {
            db = _db;
            webHostEnvironment = _webHostEnvironment;
            // Ruta donde se guarda temporalmente la información del préstamo
            ruta = Path.Combine(webHostEnvironment.WebRootPath, "Prestamos", "nuevoPrestamo.json");
        }

        /// <summary>
        /// Página principal de préstamos - muestra información del cliente si se proporciona ID
        /// </summary>
        public IActionResult Index(int? id)
        {
            // Buscar cliente si se proporciona un ID
            Models.Cliente? cliente = id != null ? db.Clientes.FirstOrDefault(c => c.IdCliente == id && c.Activo == true) : null;
            return View(cliente);
        }

        /// <summary>
        /// Muestra la lista de clientes para seleccionar en un préstamo
        /// </summary>
        public ActionResult clientes(string returnTo = "Index")
        {
            ViewBag.ReturnTo = returnTo;
            // Obtener todos los clientes activos
            IEnumerable<Models.Cliente> listaClientes = db.Clientes.Where(c => c.Activo == true).ToList();
            return View(listaClientes);
        }

        /// <summary>
        /// Selecciona un cliente específico para el préstamo
        /// </summary>
        public ActionResult SeleccionarCliente(int id, string returnTo = "Index")
        {
            var cliente = db.Clientes.Find(id);
            if (cliente != null)
            {
                // Guardar el cliente seleccionado en la sesión del préstamo
                nuevoPrestamo.cliente = cliente;
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                nuevoPrestamo.usuario = db.Usuarios.FirstOrDefault(u => u.Id == userId);       
                string json = JsonSerializer.Serialize(nuevoPrestamo);
                
                // Asegurar que el directorio existe
                var directory = Path.GetDirectoryName(ruta);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                System.IO.File.WriteAllText(ruta, json);

                // Redireccionar según el origen
                switch (returnTo.ToLower())
                {
                    case "create":
                        return RedirectToAction(nameof(Create));
                    case "tablaprestamos":
                        return RedirectToAction(nameof(tablaPrestamos), new { IdCliente = id });
                    case "libros":
                        return RedirectToAction(nameof(libros), new { idCliente = id });
                    default:
                        return RedirectToAction(nameof(Index), new { id = id });
                }
            }
            return RedirectToAction(nameof(clientes), new { returnTo = returnTo });
        }


        public ActionResult Create()
        {
            try
            {
                if (!System.IO.File.Exists(ruta))
                {
                    return RedirectToAction(nameof(clientes));
                }

                string json = System.IO.File.ReadAllText(ruta);
                nuevoPrestamo = JsonSerializer.Deserialize<Models.ViewModels.PrestamoCreate>(json);
                
                if (nuevoPrestamo == null)
                {
                    return RedirectToAction(nameof(clientes));
                }

                // Recargar referencias de Libro
                if (nuevoPrestamo.copiasLibro != null)
                {
                    foreach (var copia in nuevoPrestamo.copiasLibro)
                    {
                        if (copia.Libro == null)
                        {
                            copia.Libro = db.Libros.FirstOrDefault(l => l.IdLibro == copia.IdLibro);
                        }
                    }
                }

                return View(nuevoPrestamo);
            }
            catch
            {
                return RedirectToAction(nameof(clientes));
            }
        }

        [HttpPost]
        public ActionResult Create(DateTime fechaLimite)
        {
            try
            {
                // Verificar que el archivo exista
                if (!System.IO.File.Exists(ruta))
                {
                    return RedirectToAction(nameof(clientes));
                }

                string json = System.IO.File.ReadAllText(ruta);
                nuevoPrestamo = JsonSerializer.Deserialize<Models.ViewModels.PrestamoCreate>(json);
                
                // Verificar que el objeto y sus propiedades no sean null
                if (nuevoPrestamo == null || nuevoPrestamo.cliente == null || nuevoPrestamo.usuario == null)
                {
                    return RedirectToAction(nameof(clientes));
                }

                // Verificar que haya al menos una copia de libro
                if (nuevoPrestamo.copiasLibro == null || !nuevoPrestamo.copiasLibro.Any())
                {
                    return RedirectToAction(nameof(Create));
                }

                // Obtener estado de préstamo de forma segura
                var estadoPrestamo = db.EstadoPrestamo.FirstOrDefault(e => e.Activo == true);
                if (estadoPrestamo == null)
                {
                    // Si no hay estados disponibles, crear uno por defecto o manejar el error
                    return RedirectToAction(nameof(Create));
                }

                Models.Prestamo prestamo = new Models.Prestamo
                {
                    IdCliente = nuevoPrestamo.cliente.IdCliente,
                    IdUsuario = nuevoPrestamo.usuario.Id,
                    IdEstadoPrestamo = estadoPrestamo.IdEstado,       
                    FechaInicio = DateTime.Now,
                    FechaLimite = fechaLimite,
                    Activo = true
                };
                var idlibro = nuevoPrestamo.copiasLibro.Select(c => c.IdLibro).FirstOrDefault();
                var copia = db.CopiasLibros.FirstOrDefault(c => c.Libro.IdLibro == idlibro
                 && c.EstadoCopiaLibro.IdEstadoCopialibro == 1 
                 && c.Activo == true);
                if (copia != null)
                {
                    copia.IdEstadoCopiaLibro = 2; // Cambiar el estado a "Prestado"
                    db.CopiasLibros.Update(copia);
                }
                else
                {
                    TempData["Error"] = "No hay copias disponibles para uno de los libros seleccionados.";
                    return RedirectToAction(nameof(Create));
                }
                db.Prestamos.Add(prestamo);
                db.SaveChanges();
                foreach(var item in nuevoPrestamo.copiasLibro)
                {
                    var cpp = new CopiaLibroPrestamo
                    {
                        IdCopiaLibro = item.IdCopiaLibro,
                        IdPrestamo = prestamo.IdPrestamo
                    };
                    db.CopiasLibrosPrestamos.Add(cpp);                    
                }
                nuevoPrestamo.cliente.CantidadPrestamosDisponibles--;
                db.Clientes.Update(nuevoPrestamo.cliente);
                db.SaveChanges();
                
                // Limpiar el archivo JSON después de crear el préstamo
                System.IO.File.Delete(ruta);
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log del error si es necesario
                return RedirectToAction(nameof(Create));
            }
        }

        public ActionResult SeleccionarLibro(int? idLibro)
        {
            if (idLibro == null)
            {
                return RedirectToAction(nameof(libros));
            }
            else
            {
                CopiaLibro copiaNueva = db.CopiasLibros
                    .Where(c => c.Activo == true && c.IdLibro == idLibro && !db.CopiasLibrosPrestamos
                    .Any(cp => cp.IdCopiaLibro == c.IdCopiaLibro && cp.Activo))
                    .FirstOrDefault();

                if (copiaNueva != null)
                {
                    string json = System.IO.File.ReadAllText(ruta);
                    nuevoPrestamo = JsonSerializer.Deserialize<Models.ViewModels.PrestamoCreate>(json);
                    nuevoPrestamo.copiasLibro.Add(copiaNueva);
                    foreach (var copia in nuevoPrestamo.copiasLibro)
                    {
                        copia.Libro = db.Libros.FirstOrDefault(l => l.IdLibro == copia.IdLibro);
                    }
                    string jsonActualizado = JsonSerializer.Serialize(nuevoPrestamo);
                    System.IO.File.WriteAllText(ruta, jsonActualizado);
                    return RedirectToAction(nameof(Create));
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }                  
            }            
        }

        public ActionResult tablaPrestamos(int? IdCliente)
        {
            var vm = new Models.ViewModels.TablaPrestamo
            {
                cliente = db.Clientes.Find(IdCliente),
                prestamos = db.Prestamos
                    .Include(p => p.estadoPrestamo)
                    .Where(p => p.IdCliente == IdCliente)
                    .ToList()
            };
            return View(vm);
        }

        public ActionResult libros(int? idCliente)
        {
            if (idCliente == null || idCliente == 0)
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Models.Cliente? cliente = db.Clientes.Find(idCliente);
                if (cliente != null)
                {
                    nuevoPrestamo.cliente = cliente;             
                    var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    nuevoPrestamo.usuario = db.Usuarios.FirstOrDefault(u => u.Id == userId);       
                    string json = JsonSerializer.Serialize(nuevoPrestamo);
                    System.IO.File.WriteAllText(ruta, json);
                    var libros = db.Libros
                        .Where(l => l.Activo == true && l.Stock > 0).ToList();
                    return View(libros);
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
        }

        public ActionResult verPrestamo(int? id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var prestamo = db.Prestamos
                .Include(p => p.cliente)
                .Include(p => p.usuario)
                .Include(p => p.estadoPrestamo)
                .FirstOrDefault(p => p.IdPrestamo == id);

            if (prestamo == null)
            {
                return RedirectToAction(nameof(Index));
            }

            // Obtener las copias de libros asociadas al préstamo
            var copiasLibros = db.CopiasLibrosPrestamos
                .Where(clp => clp.IdPrestamo == id && clp.Activo)
                .Include(clp => clp.CopiaLibro)
                .ThenInclude(cl => cl!.Libro)
                .Select(clp => clp.CopiaLibro)
                .Where(cl => cl != null)
                .ToList();

            // Crear un ViewModel o usar ViewBag para pasar la información adicional
            ViewBag.CopiasLibros = copiasLibros;

            return View(prestamo);
        }

        [HttpPost]
        public ActionResult EliminarPrestamo(int id)
        {
            try
            {
                // Buscar el préstamo
                var prestamo = db.Prestamos.Find(id);
                if (prestamo == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                // Obtener todas las copias de libros asociadas al préstamo
                var copiasLibrosPrestamo = db.CopiasLibrosPrestamos
                    .Where(clp => clp.IdPrestamo == id)
                    .ToList();

                // Eliminar todos los registros de CopiasLibrosPrestamos asociados
                foreach (var copiaLibroPrestamo in copiasLibrosPrestamo)
                {
                    db.CopiasLibrosPrestamos.Remove(copiaLibroPrestamo);
                }

                // Eliminar el préstamo
                db.Prestamos.Remove(prestamo);

                // Guardar todos los cambios
                db.SaveChanges();

                // Redireccionar a la tabla de préstamos del cliente o a Index si no hay cliente
                if (prestamo.IdCliente != null)
                {
                    return RedirectToAction(nameof(tablaPrestamos), new { IdCliente = prestamo.IdCliente });
                }
                else
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception)
            {
                // En caso de error, redirigir de vuelta a la vista del préstamo
                return RedirectToAction(nameof(verPrestamo), new { id = id });
            }
        }
    }
}
