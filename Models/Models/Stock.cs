using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class Stock
    {
        [Key]
        public int? IdStock { get; set; }

        [Required(ErrorMessage = "Se necesita la Cantidad")]
        [Range(1, int.MaxValue)]
        public int? Cantidad { get; set; }

        public bool? Activo { get; set; }
    }
}
