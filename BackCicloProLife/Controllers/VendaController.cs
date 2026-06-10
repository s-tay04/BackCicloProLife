using Microsoft.AspNetCore.Mvc;
using BackCicloProLife.Data;
using BackCicloProLife.Models;
using Microsoft.AspNetCore.Http;

namespace BackCicloProLife.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VendaController : ControllerBase
    {
        private readonly Context _context; // Liga o controller ao banco de dados

        public VendaController(Context context)
        {
            _context = context;
        }

        // CADASTRAR VENDA
        [HttpPost("cadastrar")]
        public IActionResult CadastrarVenda(Venda venda)
        {
            var sessao = HttpContext.Session.GetString("IdLogado");

            if (sessao == null)
            {
                return Unauthorized("Realize login para continuar.");
            }

            // Valida se a quantidade é maior que zero
            if (venda.Quantidade <= 0)
            {
                return BadRequest("A quantidade deve ser maior que zero.");
            }

            // Valida se o total diário é maior que zero
            if (venda.TotalDiario <= 0)
            {
                return BadRequest("O total diário deve ser maior que zero.");
            }

            // Verifica se a receita informada existe
            if (venda.FkReceita.HasValue && venda.FkReceita.Value > 0)
            {
                var receitaExiste = _context.receita.Any(r => r.IdReceita == venda.FkReceita);

                if (!receitaExiste)
                {
                    return BadRequest("A receita informada não existe.");
                }
            }

            _context.venda.Add(venda);
            _context.SaveChanges();

            return Created("", venda);
        }

        // BUSCAR VENDAS
        [HttpGet("buscar")]
        public IActionResult BuscarVendas([FromQuery] int? idReceita)
        {
            var sessao = HttpContext.Session.GetString("IdLogado");

            if (sessao == null)
            {
                return Unauthorized("Realize login para continuar.");
            }

            var query = _context.venda.AsQueryable();

            // Filtra as vendas pela receita, caso o usuário informe o idReceita
            if (idReceita.HasValue)
            {
                query = query.Where(v => v.FkReceita == idReceita);
            }

            var resultado = query.ToList();

            return Ok(resultado);
        }

        // ATUALIZAR VENDA
        [HttpPut("atualizar")]
        public IActionResult AtualizarVenda(int id, Venda venda)
        {
            var sessao = HttpContext.Session.GetString("IdLogado");

            if (sessao == null)
            {
                return Unauthorized("Realize login para continuar.");
            }

            var vendaDoBanco = _context.venda.Find(id);

            if (vendaDoBanco == null)
            {
                return NotFound("Venda não encontrada.");
            }

            if (venda.Quantidade <= 0)
            {
                return BadRequest("A quantidade deve ser maior que zero.");
            }

            if (venda.TotalDiario <= 0)
            {
                return BadRequest("O total diário deve ser maior que zero.");
            }

            // Verifica se a receita informada existe
            if (venda.FkReceita.HasValue && venda.FkReceita.Value > 0)
            {
                var receitaExiste = _context.receita.Any(r => r.IdReceita == venda.FkReceita);

                if (!receitaExiste)
                {
                    return BadRequest("A receita informada não existe.");
                }
            }

            // Atualiza os dados da venda
            vendaDoBanco.Data = venda.Data;
            vendaDoBanco.Quantidade = venda.Quantidade;
            vendaDoBanco.TotalDiario = venda.TotalDiario;
            vendaDoBanco.FkReceita = venda.FkReceita;

            _context.SaveChanges();

            return Ok("Venda atualizada com sucesso.");
        }

        // DELETAR VENDA
        [HttpDelete("deletar/{id}")]
        public IActionResult DeletarVenda(int id)
        {
            var sessao = HttpContext.Session.GetString("IdLogado");

            if (sessao == null)
            {
                return Unauthorized("Realize login para continuar.");
            }

            var venda = _context.venda.Find(id);

            if (venda == null)
            {
                return NotFound("Venda não encontrada.");
            }

            _context.venda.Remove(venda);
            _context.SaveChanges();

            return Ok("Venda deletada!");
        }
    }
}