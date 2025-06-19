using System.ComponentModel.DataAnnotations;

namespace Proyecto_Periodo_2.Models
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
