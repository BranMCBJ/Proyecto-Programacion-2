using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class EstadoPrestamo
    {
        [Key]
        public int? _IdEstado { get; set; }
        [StringLength(20)]
        public string? _Nombre { get; set; }        
        [StringLength(100)]
        public string? _Descripcion { get; set; }
        public bool? _Activo { get; set; }
    }
}
