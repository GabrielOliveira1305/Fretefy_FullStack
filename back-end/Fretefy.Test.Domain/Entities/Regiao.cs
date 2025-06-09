using System;
using System.Collections.Generic;

namespace Fretefy.Test.Domain.Entities
{
    public class Regiao : IEntity
    {
        public Regiao()
        {
        }

        public Regiao(string nome, bool status)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Status = status;
        }

        public Guid Id { get; set; }
        public string Nome { get; set; }
        public bool Status { get; set; }

        public ICollection<RegiaoCidade> RegiaoCidades { get; set; }
    }
}
