using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Data;
using AspNetCoreGeneratedDocument;
using Models.ViewModels;
using System.Text.Json;

namespace Proyecto_Periodo_2.Controllers
{
    [Authorize] // Requiere autenticación para todo el controlador
    public class PrestamoController : Controller
    {
        private readonly AppDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private string ruta;
        private Models.ViewModels.PrestamoCreate nuevoPrestamo = new Models.ViewModels.PrestamoCreate()
        {
            copiasLibro = new List<CopiaLibro>()
        };

        public PrestamoController(AppDbContext _db, IWebHostEnvironment _webHostEnvironment)
        {
            db = _db;
            webHostEnvironment = _webHostEnvironment;
            ruta = Path.Combine(webHostEnvironment.WebRootPath, "Prestamos", "nuevoPrestamo.json");
        }

        public IActionResult Index(int? id)
        {
            Models.Cliente? cliente = id != null ? db.Clientes.FirstOrDefault(c => c.IdCliente == id && c.Activo == true) : null;
            return View(cliente);
        }

        public ActionResult clientes()
        {
            IEnumerable<Models.Cliente> listaClientes = db.Clientes.Where(c => c.Activo == true).ToList();
            return View(listaClientes);
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
                var estadoPrestamo = db.EstadoPrestamo.FirstOrDefault(e => e._Activo == true);
                if (estadoPrestamo == null)
                {
                    // Si no hay estados disponibles, crear uno por defecto o manejar el error
                    return RedirectToAction(nameof(Create));
                }

                Models.Prestamo prestamo = new Models.Prestamo
                {
                    IdCliente = nuevoPrestamo.cliente.IdCliente,
                    IdUsuario = nuevoPrestamo.usuario.Id,
                    IdEstadoPrestamo = estadoPrestamo._IdEstado,       
                    FechaInicio = DateTime.Now,
                    FechaLimite = fechaLimite,
                    Activo = true
                };
                
                db.Prestamos.Add(prestamo);
                db.SaveChanges();
                
                // Limpiar el archivo JSON después de crear el préstamo
                System.IO.File.Delete(ruta);
                
                return RedirectToAction(nameof(clientes));
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
    }
}
