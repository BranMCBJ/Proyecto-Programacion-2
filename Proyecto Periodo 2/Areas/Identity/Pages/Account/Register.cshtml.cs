// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Utilities;
using Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using System.Security.Claims;

namespace Proyecto_Periodo_2.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Usuario> _signInManager;
        private readonly UserManager<Usuario> _userManager;
        private readonly IUserStore<Usuario> _userStore;
        private readonly IUserEmailStore<Usuario> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RegisterModel(
            UserManager<Usuario> userManager,
            IUserStore<Usuario> userStore,
            SignInManager<Usuario> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            RoleManager<IdentityRole> roleManager,
            IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _userStore = userStore;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _emailStore = GetEmailStore();
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [Display(Name = "Nombre")]
            public string Nombre { get; set; }

            [Required]
            [Display(Name = "Primer Apellido")]
            public string Apellido1 { get; set; }

            [Required]
            [Display(Name = "Segundo Apellido")]
            public string Apellido2 { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Teléfono")]
            public string Telefono { get; set; }

            [Required]
            [Display(Name = "Cédula")]
            public string Cedula { get; set; }

            [Required]
            [Display(Name = "Nombre de Usuario")]
            public string NombreUsuario { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Contraseña")]
            public string Contrasena { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            /// 
            //Este atributo es momentaneo solo se usa para validar la contraseña no se guarda en la DB
            [DataType(DataType.Password)]
            [Display(Name = "Confirmar Contraseña")]
            [Compare("Contrasena", ErrorMessage = "Las contraseñas no coiciden.")]
            public string ConfirmarContrasena { get; set; }

            //imagen del usuario *OPCIONAL*
            [Display(Name = "UrlImagen")]
            public string UrlImagen { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!await _roleManager.RoleExistsAsync(WC.AdminRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(WC.AdminRole));
            }
            if (!await _roleManager.RoleExistsAsync(WC.UserRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(WC.UserRole));
            }
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            Debug.WriteLine("Entra al método");
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    var ImageFile = files[0];

                    // Validar tipo de archivo
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    var extension = Path.GetExtension(ImageFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(extension))
                    {
                        TempData["Error"] = "Solo se permiten archivos de imagen (jpg, jpeg, png, gif, bmp).";
                        return Page(); // O Redirect si quieres redirigir
                    }

                    // Validar tamaño máximo 5MB
                    if (ImageFile.Length > 5 * 1024 * 1024)
                    {
                        TempData["Error"] = "El archivo de imagen no puede superar los 5MB.";
                        return Page();
                    }

                    // Ruta para guardar imagen dentro de wwwroot/Images/Usuario
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string upload = Path.Combine(webRootPath, "Images", "Usuario");

                    if (!Directory.Exists(upload))
                    {
                        Directory.CreateDirectory(upload);
                    }

                    string fileName = Guid.NewGuid().ToString() + extension; //Nombre del archivo
                    string filePath = Path.Combine(upload, fileName);// direccionde las carpetas con la del archivo

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    // Asignar nombre de archivo guardado a Input.UrlImagen (solo el nombre, no ruta completa)
                    Input.UrlImagen = fileName;
                }
                Debug.WriteLine("ModelState no exploto");

                var user = new Usuario()
                {
                    Nombre = Input.Nombre,
                    Apellido1 = Input.Apellido1,
                    Apellido2 = Input.Apellido2,
                    NombreUsuario = Input.NombreUsuario,
                    UserName = Input.NombreUsuario,
                    Email = Input.Email,
                    PhoneNumber = Input.Telefono,
                    Cedula = Input.Cedula,
                    UrlImagen = Input.UrlImagen,
                    Activo = true
                };
                var result = await _userManager.CreateAsync(user, Input.Contrasena); //aca se crea el usuario con la contraseña

                if (result.Succeeded)
                {
                    Debug.WriteLine("No esplota");
                    if (User.IsInRole(WC.AdminRole))
                    {
                        await _userManager.AddToRoleAsync(user, WC.AdminRole);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, WC.UserRole);
                    }
                    // Usuario creado exitosamente
                    var claims = new List<Claim>
                {
                    new Claim("Nombre", Input.Nombre),
                    new Claim("NombreUsuario", Input.NombreUsuario),
                    new Claim("Apellido1", Input.Apellido1),
                    new Claim("Apellido2", Input.Apellido2),
                    new Claim("Cedula", Input.Cedula),
                    new Claim("UrlImagen", Input.UrlImagen ?? string.Empty)
                };
                    await _userManager.AddClaimsAsync(user, claims); //agrega los claims al usuario
                    return Redirect("~/Identity/Account/Login?register=true"); // Redirige al login con un mensaje de éxito
                }
                else
                {
                    Debug.WriteLine("Si esplota");
                    foreach (var error in result.Errors)
                    {
                        Debug.WriteLine($"Error: {error.Description}");
                    }
                }
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }

        private Usuario CreateUser()
        {
            try
            {
                return Activator.CreateInstance<Usuario>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(Usuario)}'. " +
                    $"Ensure that '{nameof(Usuario)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<Usuario> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<Usuario>)_userStore;
        }
    }
}
