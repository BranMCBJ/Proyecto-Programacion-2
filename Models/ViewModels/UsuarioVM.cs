using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class UsuarioVM
    {
        public Usuario? usuario { get; set; }
        public string? rol { get; set; }

        public string? contrasena { get; set; }
    }
}
