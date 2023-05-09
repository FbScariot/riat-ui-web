﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RIAT.DAL.Entity.Models
{
    public partial class Pessoa
    {
        public Pessoa()
        {
            PessoaTelefones = new HashSet<PessoaTelefone>();
        }

        public int Id { get; set; }
        public int? EndeIdEndereco { get; set; }
        public string AspuIdAspnetuser { get; set; }
        public string CdCpf { get; set; }
        public DateTime? DtNascimento { get; set; }
        public string CdSexo { get; set; }
        public string MenorDeIdade { get; set; }
        public string CdCpfResponsavel { get; set; }
        public string NomeResponsavel { get; set; }
        public string LinkSelfieResponsavel { get; set; }

        public virtual AspNetUser AspuIdAspnetuserNavigation { get; set; }
        public virtual Endereco EndeIdEnderecoNavigation { get; set; }
        public virtual Paciente Paciente { get; set; }
        public virtual Profissional Profissional { get; set; }
        public virtual ICollection<PessoaTelefone> PessoaTelefones { get; set; }
    }
}