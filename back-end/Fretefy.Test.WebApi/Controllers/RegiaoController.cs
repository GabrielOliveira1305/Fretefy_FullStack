using Fretefy.Test.Domain.Entities;
using Fretefy.Test.Domain.Interfaces;
using Fretefy.Test.Domain.Interfaces.Services;
using Fretefy.Test.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fretefy.Test.WebApi.Controllers
{
    [Route("api/regiao")]
    [ApiController]
    public class RegiaoController : Controller
    {
        private readonly IRegiaoService _regiaoService;

        public RegiaoController(IRegiaoService regiaoService)
        {
            _regiaoService = regiaoService;
        }

        [HttpGet]
        public IActionResult List()
        {
            IEnumerable<Regiao> regioes;

            regioes = _regiaoService.List();

            return Ok(regioes);
        }

        [HttpGet("nome")]
        public ActionResult<bool> ValidacaoNome([FromQuery] string nome, [FromQuery] Guid id)
        {
            bool result;

            if (id == Guid.Empty)
            {
                result = _regiaoService.ValidacaoNome(nome);
            }
            else
            {
                result = _regiaoService.ValidacaoNomeEditar(nome, id);
            }            

            return Ok(result);
        }

        [HttpGet("id")]
        public IActionResult ListaCidadesRegiao([FromQuery] Guid id)
        {
            IEnumerable<Cidade> regioes;

            regioes = _regiaoService.ListaCidadesRegiao(id);

            return Ok(regioes);

        }

        [HttpPost]
        public async Task<IActionResult> InsertUpdate([FromBody] RegiaoRequestDto request)
        {
            var regiao = request.Regiao;
            var idCidades = request.IdCidades;

            bool result;

            if(regiao.Id == Guid.Empty)
            {
                result = await _regiaoService.InsertRegiao(regiao, idCidades);
            }
            else if (idCidades.Count == 0)
            {
                result = await _regiaoService.UpdateRegiao(regiao);
            }
            else
            {
                result = await _regiaoService.UpdateRegiaoCompleto(regiao, idCidades);
            }

            if (result)
                return Ok(new { result = true, Message = "Operação realizada com sucesso." });
            else
                return BadRequest(new { result = false, Message = "Falha ao inserir/atualizar região." });
        }

        public class RegiaoRequestDto
        {
            public Regiao Regiao { get; set; }
            public List<Guid> IdCidades { get; set; }
            public List<Cidade> Cidades { get; set; }
        }
    }
}
