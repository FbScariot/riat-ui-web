﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RIAT.DAL.Entity.Models
{
    public partial class AlternativaAdl
    {
        public AlternativaAdl()
        {
            AlternativaAtendimentos = new HashSet<AlternativaAtendimento>();
        }

        public int Id { get; set; }
        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public string TipoConceito { get; set; }

        public virtual ICollection<AlternativaAtendimento> AlternativaAtendimentos { get; set; }
    }
}