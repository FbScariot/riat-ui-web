﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RIAT.DAL.Entity.Models
{
    public partial class Especialidade
    {
        public Especialidade()
        {
            ProfissionalEspecialidades = new HashSet<ProfissionalEspecialidade>();
        }

        public int Id { get; set; }
        public string EspeCdEspecialidade { get; set; }
        public string EspeNmEspecialidade { get; set; }
        public string EspeDsEspecialidade { get; set; }
        public string EspeStAtivo { get; set; }

        public virtual ICollection<ProfissionalEspecialidade> ProfissionalEspecialidades { get; set; }
    }
}