using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Prestamo
    {
        [Key]
        public int? IdPrestamo { get; set; }


        public int? IdCliente { get; set; }

        [ForeignKey(nameof(IdCliente))]
        public Cliente? cliente { get; set; }


        public string? IdUsuario { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public Usuario? usuario { get; set; }
        public int? IdEstadoPrestamo { get; set; }

        [ForeignKey(nameof(IdEstadoPrestamo))]
        public EstadoPrestamo? estadoPrestamo { get; set; }
        public DateTime? FechaInicio { get; set; }

        public DateTime? FechaLimite { get; set; }

        public bool? Activo { get; set; }
    }
}

