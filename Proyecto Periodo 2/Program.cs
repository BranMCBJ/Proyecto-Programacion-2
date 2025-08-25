// Importación de namespaces necesarios
using Microsoft.EntityFrameworkCore;
using Data;
using Microsoft.AspNetCore.Identity;
using Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios para la aplicación
builder.Services.AddControllersWithViews();

// Configuración de la base de datos con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
     options.UseSqlServer(
         builder.Configuration.GetConnectionString("DefaultConnection")));

// Configuración de Identity con el modelo de usuario personalizado
builder.Services.AddDefaultIdentity<Usuario>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

// Configuración de rutas de autenticación
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Configuración de políticas de contraseña más flexibles
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
});

// Configuración de sesiones
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession(Options =>
{
    Options.IdleTimeout = TimeSpan.FromMinutes(10);
    Options.Cookie.HttpOnly = true;
    Options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configuración del pipeline de la aplicación
if (!app.Environment.IsDevelopment())
{
    // Manejo de errores en producción
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Middleware para redirección HTTPS
app.UseHttpsRedirection();

// Configuración de archivos estáticos (CSS, JS, imágenes)
app.UseStaticFiles();

// Configuración de enrutamiento
app.UseRouting();

// Configuración de sesiones
app.UseSession();

// Middleware de autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Mapeo de páginas Razor (para Identity)
app.MapRazorPages();

// Configuración de rutas por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Iniciar la aplicación
app.Run();
