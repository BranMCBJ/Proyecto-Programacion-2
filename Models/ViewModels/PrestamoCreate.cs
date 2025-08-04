using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class PrestamoCreate
    {
        public Models.Cliente cliente { get; set; }
        public Usuario usuario { get; set; }
        public List<Models.CopiaLibro> copiasLibro { get; set; }
    }
}
