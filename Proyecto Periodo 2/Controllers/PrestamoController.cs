using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Data;
using AspNetCoreGeneratedDocument;
using Models.ViewModels;
using Cliente = Models.Cliente;

namespace Proyecto_Periodo_2.Controllers
{
    [Authorize] // Requiere autenticación para todo el controlador
    public class PrestamoController : Controller
    {
        private readonly AppDbContext db;
        private readonly IWebHostEnvironment webHostEnvironment;
        private Models.ViewModels.PrestamoCreate nuevoPrestamo = new Models.ViewModels.PrestamoCreate()
        {
            copiasLibro = new List<CopiaLibro>()
        };

        public PrestamoController(AppDbContext _db, IWebHostEnvironment _webHostEnvironment)
        {
            db = _db;
            webHostEnvironment = _webHostEnvironment;
        }

        public IActionResult Index(int? id)
        {
            Cliente? cliente = id != null ? db.Clientes.FirstOrDefault(c => c.IdCliente == id && c.Activo == true) : null;
            return View(cliente);
        }

        public ActionResult clientes()
        {
            IEnumerable<Cliente> listaClientes = db.Clientes.Where(c => c.Activo == true).ToList();
            return View(listaClientes);
        }


        public ActionResult Create()
        {
            return View(nuevoPrestamo);
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
                .Where(c => c.Activo == true && c.IdLibro == idLibro && !db.CopiasLibrosPrestamos.Any(cp => cp.IdCopiaLibro == c.IdCopiaLibro && cp.Activo))
                .FirstOrDefault();

                if (copiaNueva != null)
                {
                    nuevoPrestamo.copiasLibro.Add(copiaNueva);
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
                Cliente? cliente = db.Clientes.Find(idCliente);
                if (cliente != null)
                {
                    nuevoPrestamo.cliente = cliente;
                    var libros = db.Libros
                    .Where(l => l.Activo == true)
                    .Include(l => l.Stock)
                    .Select(l => new Models.ViewModels.PrestamoLibro(l)
                    {
                        Disponible = l.Stock != null && l.Stock.Cantidad > 0
                    })
                    .ToList();
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
