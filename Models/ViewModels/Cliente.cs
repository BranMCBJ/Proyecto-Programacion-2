﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class Cliente : Models.Cliente
    {
        public string nombreCompleto => $"{Nombre} {Apellido1} {Apellido2}";
    }
}
