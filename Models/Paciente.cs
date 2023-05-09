﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace RIAT.DAL.Entity.Models
{
    public partial class Paciente
    {
        public Paciente()
        {
            AtendimentoPlantaos = new HashSet<AtendimentoPlantao>();
            Atendimentos = new HashSet<Atendimento>();
            DoencaPacientes = new HashSet<DoencaPaciente>();
        }

        public int IdPessoa { get; set; }
        public int? PaciTipoDocumento { get; set; }
        public string PaciNrDocumento { get; set; }
        public int? PaciStEstadoCivil { get; set; }
        public string PaciNmMae { get; set; }
        public string PaciNmPai { get; set; }
        public string PaciNmProfissao { get; set; }
        public string PaciTxEncaminhadoPor { get; set; }
        public string PaciStAtivo { get; set; }
        public DateTime PaciDtAtivacao { get; set; }
        public DateTime? PaciDtDesativacao { get; set; }
        public int? GrinIdGrauInstrucao { get; set; }
        public int? PaciTpContratacao { get; set; }
        public int? ProfIdProfissionalResponsavel { get; set; }
        public string PaciContatoReferencia { get; set; }
        public string PaciMotivoConsulta { get; set; }

        public virtual GrauInstrucao GrinIdGrauInstrucaoNavigation { get; set; }
        public virtual Pessoa IdPessoaNavigation { get; set; }
        public virtual TipoContratacao PaciTpContratacaoNavigation { get; set; }
        public virtual Profissional ProfIdProfissionalResponsavelNavigation { get; set; }
        public virtual ICollection<AtendimentoPlantao> AtendimentoPlantaos { get; set; }
        public virtual ICollection<Atendimento> Atendimentos { get; set; }
        public virtual ICollection<DoencaPaciente> DoencaPacientes { get; set; }
    }
}