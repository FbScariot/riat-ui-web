﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RIAT.DAL.Entity.Models
{
    public partial class GrauInstrucao
    {
        public GrauInstrucao()
        {
            Pacientes = new HashSet<Paciente>();
        }

        public int Id { get; set; }
        public string GrinNmGrauInstrucao { get; set; }

        public virtual ICollection<Paciente> Pacientes { get; set; }
    }
}