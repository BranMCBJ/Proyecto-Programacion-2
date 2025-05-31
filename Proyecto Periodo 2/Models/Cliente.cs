using System.ComponentModel.DataAnnotations;

namespace Proyecto_Periodo_2.Models
{
    public class Cliente
    {
        [Key]
        public int? IdCliente { get; set; }
        [StringLength(10)]
        public string? Cedula { get; set; }
        [StringLength(50)]
        public string? Nombre { get; set; }
        [StringLength(50)]
        public string? Apellido1 { get; set; }
        [StringLength(50)]
        public string? Apellido2 { get; set; }
        [StringLength(50)]
        public string? Correo { get; set; }
        [StringLength(15)]
        public string? Telefono { get; set; }
        public int? CantidadPrestamosDisponibles { get; set; }
        public bool? Activo { get; set; }
    }
}
