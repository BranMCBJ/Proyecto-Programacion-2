using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Periodo_2.Models
{
    public class Libro
    {
        [Key]
        public int? IdLibro { get; set; }

        public int? IdStock { get; set; }
        [ForeignKey(nameof(IdStock))]
        public Stock? Stock { get; set; }

        public int? ClasificacionEdad { get; set; }

        public DateTime? FechaPublicacion { get; set; }

        [StringLength(13)]
        public string? ISBN { get; set; }

        [StringLength(50)]
        public string? Titulo { get; set; }

        [StringLength(400)]
        public string? Descripcion { get; set; }

        public bool? Activo { get; set; }
    }
}
