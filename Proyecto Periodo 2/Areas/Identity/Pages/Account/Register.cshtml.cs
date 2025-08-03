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

namespace Proyecto_Periodo_2.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            IUserStore<IdentityUser> userStore,
            SignInManager<IdentityUser> signInManager,
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
<<<<<<< HEAD
=======
            _webHostEnvironment = webHostEnvironment;
>>>>>>> 0b0e43f (Login completo)
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
                // Manejo de la imagen

                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string upload = Path.Combine(webRootPath, WC.ImagenUsuario.TrimStart('\\'));
                    Directory.CreateDirectory(upload);

                    string fileName = Guid.NewGuid().ToString();
                    string extension = Path.GetExtension(files[0].FileName);
                    string fullPath = Path.Combine(upload, fileName + extension);

                    // Guardar archivo en disco
                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }

                    //ruta relativa de UrlImagen
                    Input.UrlImagen = Path.Combine(WC.ImagenUsuario.TrimStart('\\'), fileName + extension);
                }
                Debug.WriteLine("ModelState no exploto");
                
                var user = new Usuario()
                {
                    Nombre = Input.Nombre,
                    Apellido1 = Input.Apellido1,
                    Apellido2 = Input.Apellido2,
                    NombreUsuario = Input.NombreUsuario,
<<<<<<< HEAD
=======
                    UserName = Input.NombreUsuario,
>>>>>>> 0b0e43f (Login completo)
                    Email = Input.Email,
                    PhoneNumber = Input.Telefono,
                    Cedula = Input.Cedula,
                    UrlImagen = Input.UrlImagen,
                    Activo = true
                };
<<<<<<< HEAD

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
=======
>>>>>>> 0b0e43f (Login completo)
                var result = await _userManager.CreateAsync(user, Input.Contrasena); //aca se crea el usuario con la contraseña

                if (result.Succeeded)
                {
<<<<<<< HEAD
                    if (User.IsInRole(WC.AdminRole))
                    {
                        await _userManager.AddToRoleAsync(user, WC.AdminRole);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, WC.UserRole);
                    }

                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
=======
                    Debug.WriteLine("No esplota");
                    if (User.IsInRole(WC.AdminRole))
>>>>>>> 0b0e43f (Login completo)
                    {
                        await _userManager.AddToRoleAsync(user, WC.AdminRole);
                    }
                    else
                    {
                        await _userManager.AddToRoleAsync(user, WC.UserRole);
                    }
                    // Usuario creado exitosamente
                    return LocalRedirect(returnUrl);
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

        private IdentityUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<IdentityUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                    $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}
