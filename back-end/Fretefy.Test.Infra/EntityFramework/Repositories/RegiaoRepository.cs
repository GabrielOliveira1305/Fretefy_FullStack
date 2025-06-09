using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fretefy.Test.Infra.EntityFramework.Repositories
{
    public class RegiaoRepository : IRegiaoRepository
    {
        private DbSet<Regiao> _dbSet;
        private readonly DbContext _dbContext;

        public RegiaoRepository(DbContext dbContext)
        {
            _dbSet = dbContext.Set<Regiao>();
            _dbContext = dbContext;
        }

        public IQueryable<Regiao> List()
        {
            return _dbSet.AsQueryable();
        }

        public bool ValidacaoNome(string nome)
        {
            return _dbSet.Any(r => r.Nome == nome);
        }

        public bool ValidacaoNomeEditar(string nome, Guid id)
        {
            return _dbSet.Any(r => r.Nome == nome && r.Id != id);
        }

        public IQueryable<Cidade> ListaCidadesRegiao(Guid id)
        {
            var cidadesIds = _dbContext.Set<RegiaoCidade>()
                .Where(rc => rc.RegiaoId == id)
                .Select(rc => rc.CidadeId);

            return _dbContext.Set<Cidade>()
                .Where(c => cidadesIds.Contains(c.Id));
        }

        public async Task<bool> InsertRegiao(Regiao regiao, List<Guid> idCidades)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                await _dbSet.AddAsync(regiao);
                await _dbContext.SaveChangesAsync();

                var regiaoId = regiao.Id;

                foreach (Guid cidadeId in idCidades)
                {
                    var regiaoCidade = new RegiaoCidade
                    {
                        RegiaoId = regiaoId,
                        CidadeId = cidadeId
                    };
                    await _dbContext.Set<RegiaoCidade>().AddAsync(regiaoCidade);
                }

                await _dbContext.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> UpdateRegiao(Regiao regiao)
        {
            try
            {
                var existingRegiao = await _dbSet.FindAsync(regiao.Id);
                if (existingRegiao == null)
                    return false;

                _dbContext.Entry(existingRegiao).CurrentValues.SetValues(regiao);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateRegiaoCompleto(Regiao regiao, List<Guid> idCidades)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                var existingRegiao = await _dbSet.FindAsync(regiao.Id);
                if (existingRegiao == null)
                    return false;

                _dbContext.Entry(existingRegiao).CurrentValues.SetValues(regiao);

                var cidadesAtuais = await _dbContext.Set<RegiaoCidade>()
                    .Where(rc => rc.RegiaoId == regiao.Id)
                    .ToListAsync();

                var cidadesParaRemover = cidadesAtuais
                    .Where(rc => !idCidades.Contains(rc.CidadeId))
                    .ToList();

                _dbContext.Set<RegiaoCidade>().RemoveRange(cidadesParaRemover);

                var cidadesExistentesIds = cidadesAtuais.Select(rc => rc.CidadeId);
                var cidadesParaAdicionar = idCidades
                    .Except(cidadesExistentesIds)
                    .Select(cidadeId => new RegiaoCidade
                    {
                        RegiaoId = regiao.Id,
                        CidadeId = cidadeId
                    });

                await _dbContext.Set<RegiaoCidade>().AddRangeAsync(cidadesParaAdicionar);

                await _dbContext.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }
    }
}
