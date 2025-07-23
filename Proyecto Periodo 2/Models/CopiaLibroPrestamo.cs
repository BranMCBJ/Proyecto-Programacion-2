using System.ComponentModel.DataAnnotations;

namespace Proyecto_Periodo_2.Models
{
    public class CopiaLibroPrestamo
    {
        [Key]
        public int? IdRelacion {  get; set; }
        [Required]
        public int? IdCopiaLibro { get; set; }
        public CopiaLibro? CopiaLibro { get; set; }
        [Required]
        public int? IdPrestamo { get; set; }
        public Prestamo? Prestamo { get; set; }
    }
}
