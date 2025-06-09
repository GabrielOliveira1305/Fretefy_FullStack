using Fretefy.Test.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fretefy.Test.Domain.Interfaces.Repositories
{
    public interface IRegiaoRepository
    {
        IQueryable<Regiao> List();
        bool ValidacaoNome(string nome);
        IQueryable<Cidade> ListaCidadesRegiao(Guid id);
        Task<bool> InsertRegiao(Regiao regiao, List<Guid> idCidades);
        Task<bool> UpdateRegiao(Regiao regiao);
        bool ValidacaoNomeEditar(string nome, Guid id);
        Task<bool> UpdateRegiaoCompleto(Regiao regiao, List<Guid> idCidades);
    }
}
