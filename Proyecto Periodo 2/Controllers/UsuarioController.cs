using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Data;
using Models;
using Microsoft.AspNetCore.Identity;
using Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Utilities;

namespace Proyecto_Periodo_2.Controllers
{
    [Authorize] // Requiere autenticación para todo el controlador
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<Usuario> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioController(AppDbContext db, UserManager<Usuario> userManager, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;
        }

        // GET: UsuarioController - Listar todos los usuarios activos
        public async Task<IActionResult> Index()
        {
            try
            {
                var usuarios = _userManager.Users.ToList();
                var lista = new List<UsuarioVM>();

                foreach (var usuario in usuarios)
                {
                    var roles = await _userManager.GetRolesAsync(usuario);
                    lista.Add(new UsuarioVM
                    {
                        usuario = usuario,
                        rol = roles.FirstOrDefault() ?? "Sin rol"
                    });
                }

                return View(lista);
            }
            catch (Exception)
            {
                return View(new List<UsuarioVM>());
            }
        }

        // GET: UsuarioController/Details/5 - Ver detalles de un usuario
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var usuario = _db.Usuarios.Find(id); //El método Find() busca una entidad por su clave primaria(id).
                if (usuario == null || usuario.Activo != true)
                {
                    return NotFound();
                }

                return View(usuario);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // GET: UsuarioController/Create - Mostrar formulario de creación
        public ActionResult Create()
        {
            return View();
        }

        // POST: UsuarioController/Create - Crear nuevo usuario
        [HttpPost]
        public async Task<IActionResult> Create(UsuarioVM usuarioVM, IFormFile ImageFile, string contrasena)
        {
            try
            {
                var usuario = usuarioVM.usuario;

                if (ModelState.IsValid)
                {
                    var usuarioExiste = _db.Usuarios
                        .FirstOrDefault(u =>
                            (u.NombreUsuario == usuario.NombreUsuario ||
                            u.Email == usuario.Email ||
                            u.PhoneNumber == usuario.PhoneNumber ||
                            u.Cedula == usuario.Cedula) &&
                            u.Activo == true);

                    if (usuarioExiste != null)
                    {
                        TempData["Error"] = "Usuario ya registrado.";
                        return RedirectToAction(nameof(Index));
                    }

                    usuario.Activo = true;

                    // ========== Manejo de Imagen ==========
                    if (ImageFile != null && ImageFile.Length > 0)
                    {
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                        var extension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(extension))
                        {
                            TempData["Error"] = "Solo se permiten archivos de imagen (jpg, jpeg, png, gif, bmp).";
                            return RedirectToAction(nameof(Index));
                        }

                        if (ImageFile.Length > 5 * 1024 * 1024)
                        {
                            TempData["Error"] = "El archivo no puede superar los 5MB.";
                            return RedirectToAction(nameof(Index));
                        }

                        string webRootPath = _webHostEnvironment.WebRootPath;
                        string upload = Path.Combine(webRootPath, "Clientes", "images");

                        if (!Directory.Exists(upload))
                            Directory.CreateDirectory(upload);

                        string fileName = Guid.NewGuid().ToString() + extension;

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName), FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        usuario.UrlImagen = fileName;
                    }

                    // ========== Crear usuario con UserManager ==========
                    var result = await _userManager.CreateAsync(usuario, contrasena);

                    if (!result.Succeeded)
                    {
                        TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                        return RedirectToAction(nameof(Index));
                    }

                    // ========== Asignar rol ==========
                    if (!string.IsNullOrEmpty(usuarioVM.rol))
                    {
                        // Verifica que el rol exista
                        if (!await _roleManager.RoleExistsAsync(usuarioVM.rol))
                        {
                            await _roleManager.CreateAsync(new IdentityRole("AdminRole"));
                            await _roleManager.CreateAsync(new IdentityRole("UserRole"));
                        }

                        await _userManager.AddToRoleAsync(usuario, usuarioVM.rol);
                    }

                    TempData["Exito"] = "Usuario creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }

                // ModelState no válido
                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                TempData["Error"] = "Datos inválidos: " + string.Join(", ", errores);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al crear el Usuario: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }


        // GET: UsuarioController/Edit/5 - Mostrar formulario de edición
        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var usuario = _db.Usuarios.Find(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                return View(usuario);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: UsuarioController/Edit/5 - Actualizar usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar si ya existe otro usuario con el mismo nombre de usuario o cédula
                    var usuarioExistente = _db.Usuarios
                        .FirstOrDefault(u => (u.UserName == usuario.UserName ||
                                            u.Cedula == usuario.Cedula) &&
                                            u.Id != usuario.Id);

                    if (usuarioExistente != null)
                    {
                        ModelState.AddModelError("", "Ya existe otro usuario con ese nombre de usuario o cédula.");
                        return View(usuario);
                    }
                    //actualizar
                    _db.Usuarios.Update(usuario);
                    _db.SaveChanges();

                    TempData["Mensaje"] = "Usuario actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                return View(usuario);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al actualizar el usuario");
                return View(usuario);
            }
        }

        // GET: UsuarioController/Delete/5 - Confirmar eliminación
        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null || id == 0)
                {
                    return NotFound();
                }

                var usuario = _db.Usuarios.Find(id);
                if (usuario == null)
                {
                    return NotFound();
                }

                return View(usuario);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // POST: UsuarioController/Delete/5 - Eliminar usuario 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                var usuario = _db.Usuarios.Find(id);
                if (usuario != null)
                {
                    //cambiar Activo a false
                    usuario.Activo = false;
                    _db.Usuarios.Update(usuario);
                    _db.SaveChanges();

                    TempData["Mensaje"] = "Usuario eliminado exitosamente";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Error al eliminar el usuario";
                return RedirectToAction(nameof(Index));
            }
        }

    }
}