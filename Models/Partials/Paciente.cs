﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
using System;
using System.Collections.Generic;

#nullable disable

namespace RIAT.DAL.Entity.Models
{
    public partial class Paciente
    {

        public string NomePaciente
        {
            get
            {
                return this.IdPessoaNavigation.AspuIdAspnetuserNavigation.CompleteName;
            }
        }

        public string Email
        {
            get
            {
                return this.IdPessoaNavigation.AspuIdAspnetuserNavigation.Email;
            }

        }
    }
}