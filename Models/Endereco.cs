﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RIAT.DAL.Entity.Models
{
    public partial class Endereco
    {
        public Endereco()
        {
            Pessoas = new HashSet<Pessoa>();
        }

        public int Id { get; set; }
        public string EndeNmLogradouro { get; set; }
        public string EndeCdNumero { get; set; }
        public string EndeNmComplemento { get; set; }
        public string EndeNmBairro { get; set; }
        public string EstaIdSigla { get; set; }
        public string EndeNmCidade { get; set; }
        public string EndeCdCep { get; set; }

        public virtual Estado EstaIdSiglaNavigation { get; set; }
        public virtual ICollection<Pessoa> Pessoas { get; set; }
    }
}