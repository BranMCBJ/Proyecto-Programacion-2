using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class CopiaLibro
    {
        [Key]
        public int? IdCopiaLibro { get; set; }

        [Required(ErrorMessage = "Se necesita el ID del libro")]
        public int? IdLibro { get; set; }

        [ForeignKey(nameof(IdLibro))]
        public Libro? Libro { get; set; }

        [Required(ErrorMessage = "Se necesita el Estado del libro")]
        public int? IdEstadoCopiaLibro { get; set; }

        [ForeignKey(nameof(IdEstadoCopiaLibro))]
        public EstadoCopiaLibro? EstadoCopiaLibro { get; set; }

        public bool? Activo { get; set; } = true;
    }
}