using System.Diagnostics;
using System.Runtime.CompilerServices;
using Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Models;

namespace Proyecto_Periodo_2.Controllers;

/// <summary>
/// Controlador principal de la aplicación - maneja la página de inicio
/// </summary>
[Authorize] // Requiere autenticación para todo el controlador
public class HomeController : Controller
{
    private readonly AppDbContext db;
    private readonly ILogger<HomeController> _logger;

    /// <summary>
    /// Constructor del controlador Home
    /// </summary>
    public HomeController(ILogger<HomeController> logger, AppDbContext _db)
    {
        _logger = logger;
        db = _db;
    }

    /// <summary>
    /// Acción para mostrar la página principal con estadísticas del sistema
    /// </summary>
    public IActionResult Index()
    {
        // Crear objeto con estadísticas del sistema
        Models.ViewModels.Home home = new Models.ViewModels.Home
        {
            cantidadLibros = db.Libros.Where(p => p.Activo == true).Count(),
            cantidadPrestamos = db.Prestamos.Where(p => p.Activo == true).Count(),
            cantidadClientes = db.Clientes.Where(p => p.Activo == true).Count(),
            cantidadUsuarios = db.Usuarios.Count()
        };
        return View(home);
    }

    /// <summary>
    /// Página de error del sistema
    /// </summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [AllowAnonymous] // Permitir acceso público a la página de error
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
