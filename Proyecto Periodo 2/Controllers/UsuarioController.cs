using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Data;
using Models;

namespace Proyecto_Periodo_2.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly AppDbContext _db;

        public UsuarioController(AppDbContext db)
        {
            _db = db;
        }

        // GET: UsuarioController - Listar todos los usuarios activos
        public ActionResult Index()
        {
            try
            {
                IEnumerable<Usuario> listaUsuarios = _db.Usuarios.Where(u => u.Activo == true);
                return View(listaUsuarios);
            }
            catch (Exception)
            {
                // En caso de error, retorna una lista vacía
                return View(new List<Usuario>());
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
        [ValidateAntiForgeryToken]
        public ActionResult Create(Usuario usuario)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Verificar si ya existe un usuario con el mismo nombre de usuario o cédula
                    var usuarioExistente = _db.Usuarios
                        .FirstOrDefault(u => u.UserName == usuario.UserName ||
                                           u.Cedula == usuario.Cedula);

                    if (usuarioExistente != null)
                    {
                        ModelState.AddModelError("", "Ya existe un usuario con ese nombre de usuario o cédula.");
                        return View(usuario);
                    }

                    //si no
                    usuario.Activo = true;

                    _db.Usuarios.Add(usuario);
                    _db.SaveChanges();

                    TempData["Mensaje"] = "Usuario creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                return View(usuario);
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error al crear el usuario");
                return View(usuario);
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