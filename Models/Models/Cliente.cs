using System.ComponentModel.DataAnnotations;

namespace Models
{
    /// <summary>
    /// Modelo que representa un cliente del sistema de biblioteca
    /// </summary>
    public class Cliente
    {
        [Key]
        public int? IdCliente { get; set; }
        
        [StringLength(10)]
        public string? Cedula { get; set; }
        
        [StringLength(50)]
        public string? Nombre { get; set; }
        
        [StringLength(50)]
        public string? Apellido1 { get; set; }
        
        [StringLength(50)]
        public string? Apellido2 { get; set; }
        
        [StringLength(50)]
        public string? Correo { get; set; }
        
        [StringLength(15)]
        public string? Telefono { get; set; }
        
        /// <summary>
        /// Cantidad de préstamos que puede realizar el cliente
        /// </summary>
        public int? CantidadPrestamosDisponibles { get; set; }
        
        /// <summary>
        /// Indica si el cliente está activo en el sistema
        /// </summary>
        public bool? Activo { get; set; }

        /// <summary>
        /// Ruta de la imagen de perfil del cliente
        /// </summary>
        [StringLength(300)]
        public string? URLImagen { get; set; }
    }
}
