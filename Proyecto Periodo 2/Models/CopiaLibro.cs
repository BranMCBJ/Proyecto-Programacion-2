using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Proyecto_Periodo_2.Models
{
    public class CopiaLibro
    {
        [Key]
        public int? IdCopiaLibro { get; set; }

        public int? IdLibro { get; set; }

        [ForeignKey(nameof(IdLibro))]
        public Libro? Libro { get; set; }

        public int? IdEstadoCopiaLibro { get; set; }
        [ForeignKey(nameof(IdEstadoCopiaLibro))]
        public EstadoPrestamo? EstadoCopiaLibro { get; set; }

        public bool? Activo { get; set; }

    }
}
