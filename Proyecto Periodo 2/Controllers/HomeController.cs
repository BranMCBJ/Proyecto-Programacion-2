using System.Diagnostics;
using System.Runtime.CompilerServices;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;

namespace Proyecto_Periodo_2.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext db;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, AppDbContext _db)
    {
        _logger = logger;
        db = _db;
    }

    [Authorize] // Solo esta acción requiere autenticación
    public IActionResult Index()
    {
        Models.ViewModels.Home home = new Models.ViewModels.Home
        {
            cantidadLibros = db.Libros.Where(p => p.Activo == true).Count(),
            cantidadPrestamos = db.Prestamos.Where(p => p.Activo == true).Count(),
            cantidadClientes = db.Clientes.Where(p => p.Activo == true).Count(),
            cantidadUsuarios = db.Usuarios.Count()
        };
        return View(home);
    }

    [AllowAnonymous] // Permitir acceso público
    public IActionResult Welcome()
    {
        // Si el usuario está autenticado, redirigir al Index
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index");
        }
        
        // Si no está autenticado, redirigir al login
        return RedirectToAction("Login", "Account", new { area = "Identity" });
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous] // Permitir acceso público a la página de error
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
