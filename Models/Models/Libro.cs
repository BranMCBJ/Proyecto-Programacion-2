using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Libro
    {
        [Key]
        public int? IdLibro { get; set; }

        [Required(ErrorMessage = "Se necesita la Clasificacion de Edad")]
        public int? ClasificacionEdad { get; set; }

        [Required(ErrorMessage = "Se necesita la fecha de publicación")]
        [DataType(DataType.Date)]
        public DateTime? FechaPublicacion { get; set; }

        [Required(ErrorMessage = "Se necesita el ISBN")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Solo se permiten números.")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "El ISBN debe ser de 13 digitos")]
        public string? ISBN { get; set; }

        [Required(ErrorMessage = "Se necesita el Titulo")]
        [StringLength(50)]
        public string? Titulo { get; set; }

        [Required(ErrorMessage = "Se necesita la Descripcion")]
        [StringLength(400)]
        public string? Descripcion { get; set; }

        public string? ImagenUrl { get; set; }

        [Required(ErrorMessage = "Se necesita una cantidad de stock")]
        public int? Stock { get; set; }

        public bool? Activo { get; set; }
    }
}
