using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class EstadoPrestamo
    {
        [Key]
        public int? IdEstado { get; set; }
        [StringLength(20)]
        public string? Nombre { get; set; }        
        [StringLength(100)]
        public string? Descripcion { get; set; }
        public bool? Activo { get; set; }
    }
}
