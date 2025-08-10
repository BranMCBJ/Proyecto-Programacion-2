using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class TablaPrestamo
    {
        public Models.Cliente cliente { get; set; }
        public List<Models.Prestamo> prestamos { get; set; }
    }
}
