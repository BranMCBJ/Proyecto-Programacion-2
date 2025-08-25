using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    /// <summary>
    /// Modelo que representa un préstamo de libros en el sistema
    /// </summary>
    public class Prestamo
    {
        [Key]
        public int? IdPrestamo { get; set; }

        /// <summary>
        /// ID del cliente que solicita el préstamo
        /// </summary>
        public int? IdCliente { get; set; }

        /// <summary>
        /// Navegación al cliente asociado al préstamo
        /// </summary>
        [ForeignKey(nameof(IdCliente))]
        public Cliente? cliente { get; set; }

        /// <summary>
        /// ID del usuario (empleado) que registra el préstamo
        /// </summary>
        public string? IdUsuario { get; set; }

        /// <summary>
        /// Navegación al usuario que registra el préstamo
        /// </summary>
        [ForeignKey(nameof(IdUsuario))]
        public Usuario? usuario { get; set; }
        
        /// <summary>
        /// ID del estado actual del préstamo
        /// </summary>
        public int? IdEstadoPrestamo { get; set; }

        /// <summary>
        /// Navegación al estado del préstamo
        /// </summary>
        [ForeignKey(nameof(IdEstadoPrestamo))]
        public EstadoPrestamo? estadoPrestamo { get; set; }
        
        /// <summary>
        /// Fecha de inicio del préstamo
        /// </summary>
        public DateTime? FechaInicio { get; set; }

        /// <summary>
        /// Fecha límite para la devolución del préstamo
        /// </summary>
        public DateTime? FechaLimite { get; set; }

        /// <summary>
        /// Indica si el préstamo está activo en el sistema
        /// </summary>
        public bool? Activo { get; set; }
    }
}

