using Fretefy.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Fretefy.Test.Domain.Interfaces.Services
{
    public interface IRegiaoService
    {
        Regiao Get(Guid id);
        IEnumerable<Regiao> List();
        bool ValidacaoNome(string nome);
        bool ValidacaoNomeEditar(string nome, Guid id);
        IEnumerable<Cidade> ListaCidadesRegiao(Guid id);
        Task<bool> InsertRegiao(Regiao regiao, List<Guid> idCidades);
        Task<bool> UpdateRegiao(Regiao regiao);
        Task<bool> UpdateRegiaoCompleto(Regiao regiao, List<Guid> idCidades);
    }
}
