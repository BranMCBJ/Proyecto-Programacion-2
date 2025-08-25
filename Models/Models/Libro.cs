using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    /// <summary>
    /// Modelo que representa un libro en el sistema de biblioteca
    /// </summary>
    public class Libro
    {
        [Key]
        public int? IdLibro { get; set; }

        /// <summary>
        /// Edad mínima recomendada para leer el libro
        /// </summary>
        [Required(ErrorMessage = "Se necesita la Clasificacion de Edad")]
        public int? ClasificacionEdad { get; set; }

        /// <summary>
        /// Fecha de publicación del libro
        /// </summary>
        [Required(ErrorMessage = "Se necesita la fecha de publicación")]
        [DataType(DataType.Date)]
        public DateTime? FechaPublicacion { get; set; }

        /// <summary>
        /// Código ISBN único del libro (13 dígitos)
        /// </summary>
        [Required(ErrorMessage = "Se necesita el ISBN")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Solo se permiten números.")]
        [StringLength(13, MinimumLength = 13, ErrorMessage = "El ISBN debe ser de 13 digitos")]
        public string? ISBN { get; set; }

        /// <summary>
        /// Título del libro
        /// </summary>
        [Required(ErrorMessage = "Se necesita el Titulo")]
        [StringLength(50)]
        public string? Titulo { get; set; }

        /// <summary>
        /// Descripción o sinopsis del libro
        /// </summary>
        [Required(ErrorMessage = "Se necesita la Descripcion")]
        [StringLength(400)]
        public string? Descripcion { get; set; }

        /// <summary>
        /// URL de la imagen de portada del libro
        /// </summary>
        public string? ImagenUrl { get; set; }

        /// <summary>
        /// Cantidad de copias disponibles del libro
        /// </summary>
        [Required(ErrorMessage = "Se necesita una cantidad de stock")]
        public int? Stock { get; set; }

        /// <summary>
        /// Indica si el libro está activo en el sistema
        /// </summary>
        public bool? Activo { get; set; }
    }
}
