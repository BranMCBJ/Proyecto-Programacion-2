using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Usuario
    {
        [Key]
        public int? IdUsuario { get; set; }
        [StringLength(50)]
        public string? NombreDeUsuario { get; set; }
        [StringLength(20)]
        public string? Contrasena { get; set; }
        [StringLength(10)]
        public string? Cedula { get; set; }
        [StringLength(50)]
        public string? Nombre { get; set; }
        [StringLength(50)]
        public string? Apellido1 { get; set; }
        [StringLength(50)]
        public string? Apellido2 { get; set; }
        public bool? Activo { get; set; }
    }
}
