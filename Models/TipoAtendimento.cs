﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RIAT.DAL.Entity.Models
{
    public partial class TipoAtendimento
    {
        public TipoAtendimento()
        {
            Atendimentos = new HashSet<Atendimento>();
        }

        public int Id { get; set; }
        public string TipaNmTipoAtendimento { get; set; }

        public virtual ICollection<Atendimento> Atendimentos { get; set; }
    }
}