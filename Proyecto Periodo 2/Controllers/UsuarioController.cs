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
using System.Security.Claims;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Proyecto_Periodo_2.Controllers
{
    [Authorize] // Requiere autenticación para todo el controlador
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _db;
        private readonly UserManager<Usuario> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<Usuario> _signInManager;

        public UsuarioController(AppDbContext db,
            UserManager<Usuario> userManager,
            IWebHostEnvironment webHostEnvironment,
            RoleManager<IdentityRole> roleManager, 
            SignInManager<Usuario> signInManager)
        {
            _db = db;
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
            _roleManager = roleManager;
            _signInManager = signInManager;
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
        public ActionResult _PartialCrearUsuario()
        {
            var usuarioVM = new UsuarioVM();
            return PartialView("_PartialCrearUsuario", usuarioVM);
        }

        // POST: UsuarioController/Create - Crear nuevo usuario
        [HttpPost]
        public async Task<IActionResult> Create(UsuarioVM usuarioVM /*, IFormFile ImageFile*/)
        {
            try
            {
                var usuario = usuarioVM.usuario;

                if (ModelState.IsValid)
                {
                    Debug.WriteLine("Modelo valido");
                    //Comprueba si el usuario existe
                    var usuarioExiste = _db.Usuarios
                        .FirstOrDefault(u =>
                            (u.NombreUsuario == usuario.NombreUsuario ||
                            u.Email == usuario.Email ||
                            u.PhoneNumber == usuario.PhoneNumber ||
                            u.Cedula == usuario.Cedula) &&
                            u.Activo == true);

                    if (usuarioExiste != null)
                    {
                        Debug.WriteLine("Usuario existe");
                        TempData["Error"] = "Usuario ya registrado.";
                        return RedirectToAction(nameof(Index));
                    }

                    usuario.Activo = true;

                    /*// ========== Manejo de Imagen ==========
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
                        string upload = Path.Combine(webRootPath, "Usuario", "Imagenes");

                        if (!Directory.Exists(upload))
                            Directory.CreateDirectory(upload);

                        string fileName = Guid.NewGuid().ToString() + extension;

                        using (var fileStream = new FileStream(Path.Combine(upload, fileName), FileMode.Create))
                        {
                            await ImageFile.CopyToAsync(fileStream);
                        }

                        usuario.UrlImagen = fileName;
                    }*/

                    //Agrego valores necesarios para Identity
                    var user = new Usuario
                    {
                        UserName = usuario.NombreUsuario,
                        Email = usuario.Email,
                        PhoneNumber = usuario.PhoneNumber,
                        Cedula = usuario.Cedula,
                        Nombre = usuario.Nombre,
                        Apellido1 = usuario.Apellido1,
                        Apellido2 = usuario.Apellido2,
                        UrlImagen = usuario.UrlImagen,
                        NombreUsuario = usuario.NombreUsuario,
                        Activo = true
                    };

                    //se crea el usuario con la contraseña hasheada
                    var result = await _userManager.CreateAsync(user, usuarioVM.contrasena);

                    if (!result.Succeeded)
                    {
                        Debug.WriteLine("No se creo el usaurio exploto");
                        TempData["Error"] = string.Join(", ", result.Errors.Select(e => e.Description));
                        return RedirectToAction(nameof(Index));
                    }

                        // Update the claims creation section to handle potential null values for "Cedula" and "UrlImagen".
                    var claims = new List<Claim>
                    {
                        new Claim("Nombre", user.Nombre ?? string.Empty),
                        new Claim("NombreUsuario", user.NombreUsuario ?? string.Empty),
                        new Claim("Apellido1", user.Apellido1 ?? string.Empty),
                        new Claim("Apellido2", user.Apellido2 ?? string.Empty),
                        new Claim("Cedula", user.Cedula ?? string.Empty),
                        new Claim("UrlImagen", user.UrlImagen ?? string.Empty)
                    };

                    // Añadir claims al usuario de manera permanente
                    await _userManager.AddClaimsAsync(user, claims);

                    // ========== Asignar rol ==========
                    if (!string.IsNullOrEmpty(usuarioVM.rol))
                    {
                        if (!await _roleManager.RoleExistsAsync("Administrador"))
                            await _roleManager.CreateAsync(new IdentityRole("Administrador"));

                        if (!await _roleManager.RoleExistsAsync("Usuario"))
                            await _roleManager.CreateAsync(new IdentityRole("Usuario"));

                        await _userManager.AddToRoleAsync(user, usuarioVM.rol);
                    }

                    TempData["Exito"] = "Usuario creado correctamente.";
                    return RedirectToAction(nameof(Index));
                }
                Debug.WriteLine("Modelo no valido");
                foreach (var modelState in ModelState)
                {
                    var key = modelState.Key;
                    foreach (var error in modelState.Value.Errors)
                    {
                        Debug.WriteLine($"Error en campo '{key}': {error.ErrorMessage} / Excepcion: {error.Exception?.Message}");
                    }
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
        public async Task<ActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("ID de usuario no proporcionado.");
            }

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null || usuario.Activo == false)
            {
                return NotFound("Usuario no encontrado");
            }

            //Datos que se muestran el modal de actualizar
            var usuarioVM = new UsuarioVM()
            {
                usuario = new Usuario()
                {
                    Nombre = usuario.Nombre,
                    Apellido1 = usuario.Apellido1,
                    Apellido2 = usuario.Apellido2,
                    NombreUsuario = usuario.NombreUsuario,
                    Email = usuario.Email,
                    PhoneNumber = usuario.PhoneNumber,
                    Cedula = usuario.Cedula,
                    UrlImagen = usuario.UrlImagen,
                    Id = usuario.Id
                },

                rol = (await _userManager.GetRolesAsync(usuario)).FirstOrDefault(),

                contrasena = string.Empty
            };

            return PartialView("_PartialEditarUsuario", usuarioVM);
        }

        // POST: UsuarioController/Edit/5 - Actualizar usuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(UsuarioVM usuarioVM)
        {
            {
                try
                {
                    var usuario = usuarioVM.usuario;


                    // Evita que se valide la contraseña si está vacía
                    ModelState.Remove(nameof(usuarioVM.contrasena));

                    if (ModelState.IsValid)
                    {

                        usuario.Activo = true;

                        // Cargar el usuario real desde la base de datos
                        var usuarioEnDb = await _db.Usuarios.FindAsync(usuario.Id);
                        if (usuarioEnDb == null || usuarioEnDb.Activo == false)
                        {
                            TempData["Error"] = "Usuario no encontrado.";
                            return RedirectToAction(nameof(Index));
                        }

                        // Actualizar datos
                        usuarioEnDb.Nombre = usuario.Nombre;
                        usuarioEnDb.Apellido1 = usuario.Apellido1;
                        usuarioEnDb.Apellido2 = usuario.Apellido2;
                        usuarioEnDb.Cedula = usuario.Cedula;
                        usuarioEnDb.NombreUsuario = usuario.NombreUsuario;
                        usuarioEnDb.PhoneNumber = usuario.PhoneNumber;
                        usuarioEnDb.Email = usuario.Email;
                        usuarioEnDb.Activo = true;

                        // Cambiar contraseña si se especifica
                        if (!string.IsNullOrWhiteSpace(usuarioVM.contrasena))
                        {

                            var token = await _userManager.GeneratePasswordResetTokenAsync(usuarioEnDb);
                            var resultado = await _userManager.ResetPasswordAsync(usuarioEnDb, token, usuarioVM.contrasena);

                            if (!resultado.Succeeded)
                            {
                                Debug.WriteLine("Error al cambiar la contraseña: " + string.Join(", ", resultado.Errors.Select(e => e.Description)));
                                TempData["Error"] = "Error al cambiar la contraseña: " + string.Join(", ", resultado.Errors.Select(e => e.Description));
                                return RedirectToAction(nameof(Index));
                            }

                        }

                        // Actualizar tabla IdentityUser
                        var resultadoUpdate = await _userManager.UpdateAsync(usuarioEnDb);

                        // Guardar cambios en la tabla extendida
                        await _db.SaveChangesAsync();

                        var idSession = User.FindFirstValue(ClaimTypes.NameIdentifier);
                        if (idSession == usuarioEnDb.Id)
                        {
                            await _signInManager.RefreshSignInAsync(usuarioEnDb);
                        }

                        // Actualizar rol
                        var rolesActuales = await _userManager.GetRolesAsync(usuarioEnDb);
                        await _userManager.RemoveFromRolesAsync(usuarioEnDb, rolesActuales);

                        if (!string.IsNullOrEmpty(usuarioVM.rol))
                        {
                            await _userManager.AddToRoleAsync(usuarioEnDb, usuarioVM.rol);
                            Debug.WriteLine($"Nuevo rol asignado: {usuarioVM.rol}");
                        }

                        TempData["Exito"] = "Usuario actualizado correctamente.";
                        return RedirectToAction(nameof(Index));
                    }

                    var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    TempData["Error"] = "Datos inválidos: " + string.Join(", ", errores);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "Error al actualizar el usuario: " + ex.Message;
                    return RedirectToAction(nameof(Index));
                }
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