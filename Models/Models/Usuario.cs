using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    /// <summary>
    /// Modelo de usuario del sistema que extiende IdentityUser para autenticación
    /// </summary>
    public class Usuario : IdentityUser
    {
        /// <summary>
        /// Número de cédula del usuario
        /// </summary>
        public string? Cedula { get; set; }
        
        /// <summary>
        /// Nombre del usuario
        /// </summary>
        [StringLength(50)]
        public string? Nombre { get; set; }
        
        /// <summary>
        /// Primer apellido del usuario
        /// </summary>
        [StringLength(50)]
        public string? Apellido1 { get; set; }
        
        /// <summary>
        /// Segundo apellido del usuario
        /// </summary>
        [StringLength(50)]
        public string? Apellido2 { get; set; }
        
        /// <summary>
        /// Indica si el usuario está activo en el sistema
        /// </summary>
        public bool? Activo { get; set; }
        
        /// <summary>
        /// URL de la imagen de perfil del usuario
        /// </summary>
        public string? UrlImagen { get; set; }
        
        /// <summary>
        /// Nombre de usuario para el login (requerido)
        /// </summary>
        [Required]
        public string? NombreUsuario { get; set; }
    }
}
