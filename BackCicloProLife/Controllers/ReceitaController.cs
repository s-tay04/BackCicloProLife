using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackCicloProLife.Data;
using BackCicloProLife.Models;
using Microsoft.AspNetCore.Http;

namespace BackCicloProLife.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ReceitaController : ControllerBase
    {
        private readonly Context _context;

        public ReceitaController(Context context)
        {
            _context = context;
        }

        // CADASTRAR
        [HttpPost("cadastrar")]
        public IActionResult CadastrarReceita(Models.Receita receita)
        {
            var sessao = HttpContext.Session.GetString("IdLogado");

            if (sessao == null)
            {
                return Unauthorized("Faça login para cadastrar uma receita.");
            }

            var idUsuario = Convert.ToInt32(sessao);

            receita.FkUsuarioColaborador = idUsuario;

            receita.DataCadastro = DateTime.Now;
            receita.Status = "Pendente";

            if (receita.ArquivoImagem != null)
            {
                var nomeArquivo = Guid.NewGuid().ToString() +
                                  Path.GetExtension(receita.ArquivoImagem.FileName);

                var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                if (!Directory.Exists(pasta))
                    Directory.CreateDirectory(pasta);

                var caminho = Path.Combine(pasta, nomeArquivo);

                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                receita.ArquivoImagem.CopyToAsync(stream);
                }

                receita.Imagem = nomeArquivo;
            }

            _context.receita.Add(receita);
            _context.SaveChanges();

            return Created("", receita);
        }

        // ATUALIZAR
        [HttpPut("atualizar")]
        public IActionResult AtualizarReceita(int id, Models.Receita receita)
        {
            var sessao = HttpContext.Session.GetString("IdLogado");
            if (sessao == null)
            {
                return Unauthorized("Realize o login para continuar.");
            }

            var receitaDoBanco = _context.receita.Find(id);

            if (receitaDoBanco == null)
                return NotFound("Receita não encontrada.");

            if (receita.FkUsuarioColaborador.HasValue && receita.FkUsuarioColaborador.Value > 0)
            {
                var colaboradorExiste = _context.usuario.Any(u => u.IdUsuario == receita.FkUsuarioColaborador);
                if (!colaboradorExiste) return BadRequest("O ID de Colaborador informado não existe.");
            }
            if (receita.FkUsuarioChefe.HasValue && receita.FkUsuarioChefe.Value > 0)
            {
                var chefeExiste = _context.usuario.Any(u => u.IdUsuario == receita.FkUsuarioChefe);
                if (!chefeExiste) return BadRequest("O ID de Chefe informado não existe.");
            }
            if (receita.FkUsuarioGestor.HasValue && receita.FkUsuarioGestor.Value > 0)
            {
                var gestorExiste = _context.usuario.Any(u => u.IdUsuario == receita.FkUsuarioGestor);
                if (!gestorExiste) return BadRequest("O ID de Gestor informado não existe.");
            }

            receitaDoBanco.Titulo = receita.Titulo;
            receitaDoBanco.Custo = receita.Custo;
            receitaDoBanco.ModoPreparo = receita.ModoPreparo;
            receitaDoBanco.Porcao = receita.Porcao;
            receitaDoBanco.Status = receita.Status;
            receitaDoBanco.FeedbackChefe = receita.FeedbackChefe;
            receitaDoBanco.FkUsuarioColaborador = receita.FkUsuarioColaborador;
            receitaDoBanco.FkUsuarioChefe = receita.FkUsuarioChefe;
            receitaDoBanco.FkUsuarioGestor = receita.FkUsuarioGestor;

            _context.SaveChanges();

            return Ok("Receita atualizada com sucesso.");
        }

        // DELETAR
        [HttpDelete("delete/{id}")]
        public IActionResult DeletarReceita(int id)
        {
            var sessao = HttpContext.Session.GetString("IdLogado");
            if (sessao == null)
            {
                return Unauthorized("Realize o login para continuar.");
            }

            var receita = _context.receita.Find(id);

            if (receita == null)
                return NotFound("Receita não encontrada.");

            _context.receita.Remove(receita);
            _context.SaveChanges();

            return Ok("Receita deletada!");
        }

        // BUSCAR
        [HttpGet("buscar")]
        public IActionResult BuscarReceitas([FromQuery] string? nome)
        {
            var sessaoUsuario = HttpContext.Session.GetString("IdLogado");
            if (sessaoUsuario == null)
            {
                return Unauthorized("Faça login antes.");
            }

            var query = _context.receita.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(r => r.Titulo.Contains(nome));
            }

            var resultado = query.ToList();

            return Ok(resultado);
        }

        // LISTAR RECEITAS (cards)
        [HttpGet("listar")]
        public IActionResult ListarReceitas()
        {
            try
            {
                var receitas = _context.receita.ToList();

                if (receitas == null || !receitas.Any())
                {
                    return Ok(new List<Models.Receita>());
                }

                return Ok(receitas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao buscar receitas: {ex.Message}");
            }
        }
    }
}