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
        #region Propiedades y Constructor
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
        #endregion

        #region Acciones de Vista Principal

        /// <summary>
        /// Página principal de préstamos - muestra información del cliente si se proporciona ID
        /// </summary>
        public IActionResult Index(int? id)
        {
            // Operador ternario: si id tiene valor busca el cliente activo sino devuelve null
            Models.Cliente? cliente = id != null ? db.Clientes.FirstOrDefault(c => c.IdCliente == id && c.Activo == true) : null;
            return View(cliente);
        }
        #endregion

        #region Gestión de Clientes
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
                // Asignar cliente seleccionado al objeto de prestamo temporal
                nuevoPrestamo.cliente = cliente;
                // Obtener ID del usuario autenticado usando Claims
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                // Buscar usuario completo en base de datos usando el ID obtenido
                nuevoPrestamo.usuario = db.Usuarios.FirstOrDefault(u => u.Id == userId);       
                // Serializar objeto a JSON para guardarlo temporalmente
                string json = JsonSerializer.Serialize(nuevoPrestamo);
                
                // Obtener directorio padre de la ruta del archivo
                var directory = Path.GetDirectoryName(ruta);
                // Crear directorio si no existe para evitar errores
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                
                // Escribir JSON al archivo temporal
                System.IO.File.WriteAllText(ruta, json);

                // Switch para redireccionar segun el parametro returnTo
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
        #endregion

        #region Creación de Préstamos
        public ActionResult Create()
        {
            try
            {
                // Verificar si existe el archivo JSON temporal con datos del prestamo
                if (!System.IO.File.Exists(ruta))
                {
                    return RedirectToAction(nameof(clientes));
                }

                // Leer contenido del archivo JSON
                string json = System.IO.File.ReadAllText(ruta);
                // Deserializar JSON a objeto PrestamoCreate
                nuevoPrestamo = JsonSerializer.Deserialize<Models.ViewModels.PrestamoCreate>(json);
                
                // Validar que la deserializacion fue exitosa
                if (nuevoPrestamo == null)
                {
                    return RedirectToAction(nameof(clientes));
                }

                // Rehidratar referencias de Libro que se perdieron en la serializacion
                if (nuevoPrestamo.copiasLibro != null)
                {
                    foreach (var copia in nuevoPrestamo.copiasLibro)
                    {
                        // Si la referencia a Libro es null la buscamos en la base de datos
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
                // En caso de error volver a seleccion de clientes
                return RedirectToAction(nameof(clientes));
            }
        }

        [HttpPost]
        public ActionResult Create(DateTime fechaLimite)
        {
            try
            {
                // Verificar que el archivo temporal con datos del prestamo exista
                if (!System.IO.File.Exists(ruta))
                {
                    return RedirectToAction(nameof(clientes));
                }

                // Leer y deserializar datos del prestamo desde archivo JSON
                string json = System.IO.File.ReadAllText(ruta);
                nuevoPrestamo = JsonSerializer.Deserialize<Models.ViewModels.PrestamoCreate>(json);
                
                // Validar que todos los objetos requeridos esten disponibles
                if (nuevoPrestamo == null || nuevoPrestamo.cliente == null || nuevoPrestamo.usuario == null)
                {
                    return RedirectToAction(nameof(clientes));
                }

                // Validar que exista al menos una copia de libro seleccionada
                if (nuevoPrestamo.copiasLibro == null || !nuevoPrestamo.copiasLibro.Any())
                {
                    return RedirectToAction(nameof(Create));
                }

                // Buscar estado de prestamo activo en la base de datos
                var estadoPrestamo = db.EstadoPrestamo.FirstOrDefault(e => e.Activo == true);
                if (estadoPrestamo == null)
                {
                    // Si no hay estados disponibles regresar al formulario
                    return RedirectToAction(nameof(Create));
                }

                // Crear nuevo objeto Prestamo con los datos recolectados
                Models.Prestamo prestamo = new Models.Prestamo
                {
                    IdCliente = nuevoPrestamo.cliente.IdCliente,
                    IdUsuario = nuevoPrestamo.usuario.Id,
                    IdEstadoPrestamo = estadoPrestamo.IdEstado,       
                    FechaInicio = DateTime.Now,
                    FechaLimite = fechaLimite,
                    Activo = true
                };
                
                // Obtener ID del primer libro seleccionado
                var idlibro = nuevoPrestamo.copiasLibro.Select(c => c.IdLibro).FirstOrDefault();
                // Buscar copia disponible: debe estar activa y en estado "Disponible" (ID=1)
                var copia = db.CopiasLibros.FirstOrDefault(c => c.Libro.IdLibro == idlibro
                 && c.EstadoCopiaLibro.IdEstadoCopialibro == 1 
                 && c.Activo == true);
                
                if (copia != null)
                {
                    // Cambiar estado de la copia a "Prestado" (ID=2)
                    copia.IdEstadoCopiaLibro = 2;
                    db.CopiasLibros.Update(copia);
                }
                else
                {
                    TempData["Error"] = "No hay copias disponibles para uno de los libros seleccionados.";
                    return RedirectToAction(nameof(Create));
                }
                
                // Guardar el prestamo en base de datos
                db.Prestamos.Add(prestamo);
                db.SaveChanges();
                
                // Crear relaciones entre copias de libros y el prestamo
                foreach(var item in nuevoPrestamo.copiasLibro)
                {
                    var cpp = new CopiaLibroPrestamo
                    {
                        IdCopiaLibro = item.IdCopiaLibro,
                        IdPrestamo = prestamo.IdPrestamo
                    };
                    db.CopiasLibrosPrestamos.Add(cpp);                    
                }
                
                // Decrementar cantidad de prestamos disponibles del cliente
                nuevoPrestamo.cliente.CantidadPrestamosDisponibles--;
                db.Clientes.Update(nuevoPrestamo.cliente);
                db.SaveChanges();
                
                // Eliminar archivo temporal ya que el prestamo fue creado exitosamente
                System.IO.File.Delete(ruta);
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // Log del error si es necesario
                return RedirectToAction(nameof(Create));
            }
        }
        #endregion

        #region Gestión de Libros en Préstamos
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
        #endregion

        #region Visualización de Préstamos
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
        #endregion

        #region Eliminación de Préstamos
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
        #endregion
    }
}