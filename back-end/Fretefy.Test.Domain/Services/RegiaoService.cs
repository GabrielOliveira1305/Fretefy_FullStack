using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces.Repositories;
using Fretefy.Test.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Fretefy.Test.Domain.Interfaces.Services;
using System.Linq;
using System.Threading.Tasks;

namespace Fretefy.Test.Domain.Services
{
    public class RegiaoService : IRegiaoService
    {
        private readonly IRegiaoRepository _regiaoRepository;

        public RegiaoService(IRegiaoRepository regiaoRepository)
        {
            _regiaoRepository = regiaoRepository;
        }

        public Regiao Get(Guid id)
        {
            return _regiaoRepository.List().FirstOrDefault(f => f.Id == id);
        }

        public IEnumerable<Regiao> List()
        {
            return _regiaoRepository.List();
        }

        public bool ValidacaoNome(string nome)
        {
            return _regiaoRepository.ValidacaoNome(nome);
        }

        public bool ValidacaoNomeEditar(string nome, Guid id)
        {
            return _regiaoRepository.ValidacaoNomeEditar(nome, id);
        }

        public IEnumerable<Cidade> ListaCidadesRegiao(Guid id)
        {
            return _regiaoRepository.ListaCidadesRegiao(id);
        }

        public Task<bool> InsertRegiao(Regiao regiao, List<Guid> idCidades)
        {
            return _regiaoRepository.InsertRegiao(regiao, idCidades);
        }

        public Task<bool> UpdateRegiao(Regiao regiao)
        {
            return _regiaoRepository.UpdateRegiao(regiao);
        }

        public Task<bool> UpdateRegiaoCompleto(Regiao regiao, List<Guid> idCidades)
        {
            return _regiaoRepository.UpdateRegiaoCompleto(regiao, idCidades);
        }
    }
}