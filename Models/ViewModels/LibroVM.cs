using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class LibroVM
    {
        public Libro? Libro { get; set; }
        public IFormFile? Imagen { get; set; }
    }
}
