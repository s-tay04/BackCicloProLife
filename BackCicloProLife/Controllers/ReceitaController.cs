using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackCicloProLife.Data;
using BackCicloProLife.Models;
using Microsoft.AspNetCore.Http;
using BackCicloProLife.DTOs;
using System.Text.Json;

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
        public IActionResult CadastrarReceita([FromForm] ReceitaCadastroDTO dto)
        {
            try
            {
                var cookie = Request.Cookies["IdLogado"];

                if (cookie == null)
                {
                    return Unauthorized("Faça login para cadastrar uma receita.");
                }

                var idUsuario = Convert.ToInt32(cookie);

                var receita = new Receita
                {
                    Titulo = dto.Titulo,
                    Custo = dto.Custo,
                    ModoPreparo = dto.ModoPreparo,
                    Porcao = dto.Porcao,
                    FkUsuarioColaborador = idUsuario,
                    DataCadastro = DateTime.Now,
                    Status = "Pendente",
                    FeedbackChefe = ""
                };

                // Salvar imagem
                if (dto.ArquivoImagem != null)
                {
                    var nomeArquivo = Guid.NewGuid().ToString() +
                                      Path.GetExtension(dto.ArquivoImagem.FileName);

                    var pasta = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads");

                    if (!Directory.Exists(pasta))
                    {
                        Directory.CreateDirectory(pasta);
                    }

                    var caminho = Path.Combine(pasta, nomeArquivo);

                    using (var stream = new FileStream(caminho, FileMode.Create))
                    {
                        dto.ArquivoImagem.CopyTo(stream);
                    }

                    receita.Imagem = nomeArquivo;
                }

                // Salva a receita
                _context.receita.Add(receita);
                _context.SaveChanges();

                Console.WriteLine("JSON recebido:");
                Console.WriteLine(dto.Ingredientes);

                // Converte o JSON em lista
                var ingredientes = JsonSerializer.Deserialize<List<IngredienteDTO>>(
                    dto.Ingredientes,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                if (ingredientes == null || ingredientes.Count == 0)
                {
                    return Created("", new
                    {
                        receita.IdReceita,
                        receita.Titulo,
                        receita.Status,
                        receita.Imagem,
                        mensagem = "Receita cadastrada com sucesso!"
                    });
                }

                Console.WriteLine($"Quantidade de ingredientes: {ingredientes.Count}");

                foreach (var item in ingredientes)
                {
                    Console.WriteLine($"ID: {item.IdIngrediente}");
                    Console.WriteLine($"Quantidade: {item.Quantidade}");
                    Console.WriteLine($"Unidade: {item.Unidade}");

                    var ingrediente = _context.ingrediente.FirstOrDefault(i =>
                        i.IdIngrediente == item.IdIngrediente);

                    if (ingrediente == null)
                    {
                        return BadRequest($"Ingrediente com ID {item.IdIngrediente} não encontrado.");
                    }

                    var ingredienteReceita = new IngredienteReceita
                    {
                        FkIngrediente = item.IdIngrediente,
                        FkReceita = receita.IdReceita,
                        Quantidade = item.Quantidade,
                        Unidade = item.Unidade
                    };

                    _context.ingredienteReceita.Add(ingredienteReceita);
                }

                _context.SaveChanges();

                return Created("", new
                {
                    receita.IdReceita,
                    receita.Titulo,
                    receita.Status,
                    receita.Imagem,
                    mensagem = "Receita cadastrada com sucesso!"
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return StatusCode(500, ex.ToString());
            }
        }

        // DESTAQUES
        [HttpGet("destaques")]
        public async Task<IActionResult> Destaques()
        {
            var receitas = await _context.receita
                .Where(r => r.Status == "Aprovada")
                .OrderBy(r => r.Custo) // menor custo primeiro
                .ThenByDescending(r => r.Porcao) // maior rendimento
                .ThenByDescending(r => r.DataCadastro) // mais recentes
                .Take(4)
                .Select(r => new
                {
                    r.IdReceita,
                    r.Titulo,
                    r.Imagem,
                    r.Custo,
                    r.Porcao,
                    r.DataCadastro
                })
                .ToListAsync();

            return Ok(receitas);
        }

        // ATUALIZAR
        [HttpPut("atualizar/{id}")]
        public IActionResult AtualizarReceita(int id, Receita receita)
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
            var cookie = Request.Cookies["IdLogado"];

            if (cookie == null)
            {
                return Unauthorized("Realize o login para continuar.");
            }

            var receita = _context.receita.Find(id);

            if (receita == null)
                return NotFound("Receita não encontrada.");

            var ingredientes = _context.ingredienteReceita
                .Where(ir => ir.FkReceita == id)
                .ToList();

            _context.ingredienteReceita.RemoveRange(ingredientes);

            _context.receita.Remove(receita);

            _context.SaveChanges();

            return Ok(new
            {
                mensagem = "Receita reprovada e removida com sucesso."
            });
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

            var query = _context.receita
                .Where(r => r.Status == "Aprovada");

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(r => r.Titulo.Contains(nome));
            }

            var resultado = query
                .Select(r => new
                {
                    r.IdReceita,
                    r.Titulo,
                    r.Imagem,
                    r.Status
                })
                .ToList();

            return Ok(resultado);
        }

        // LISTAR RECEITAS (cards)
        // Colaborador
        [HttpGet("pendentes")]
        public async Task<IActionResult> ReceitasPendentes()
        {
            var receitas = await _context.receita
                .Where(r => r.Status == "Pendente")
                .Select(r => new
                {
                    r.IdReceita,
                    r.Titulo,
                    r.Imagem,
                    r.Status,
                    r.FeedbackChefe,
                    r.DataCadastro
                })
                .ToListAsync();

            return Ok(receitas);
        }

        // Chef
        [HttpGet("fase-teste")]
        public async Task<IActionResult> FaseTeste()
        {
            var receitas = await _context.receita
                .Where(r => r.Status == "Pendente")
                .Select(r => new
                {
                    r.IdReceita,
                    r.Titulo,
                    r.Imagem,
                    r.Status,
                    r.FeedbackChefe,
                    r.DataCadastro
                })
                .ToListAsync();

            return Ok(receitas);
        }

        // Gestor
        [HttpGet("fase-final")]
        public async Task<IActionResult> FaseFinal()
        {
            var receitas = await _context.receita
                .Where(r => r.Status == "Gestor")
                .Select(r => new
                {
                    r.IdReceita,
                    r.Titulo,
                    r.Imagem,
                    r.Status,
                    r.FeedbackChefe,
                    r.DataCadastro
                })
                .ToListAsync();

            return Ok(receitas);
        }

        // Cardápio
        [HttpGet("cardapio")]
        public async Task<IActionResult> Cardapio()
        {
            var receitas = await _context.receita
                .Where(r => r.Status == "Aprovada")
                .Select(r => new
                {
                    r.IdReceita,
                    r.Titulo,
                    r.Imagem,
                    r.Status,
                    r.FeedbackChefe,
                    r.DataCadastro
                })
                .ToListAsync();

            return Ok(receitas);
        }

        // BUSCAR UMA RECEITA
        [HttpGet("{id}")]
        public IActionResult BuscarReceita(int id)
        {
            var receita = _context.receita
                .Include(r => r.IngredientesReceita)
                    .ThenInclude(ir => ir.Ingrediente)
                .FirstOrDefault(r => r.IdReceita == id);

            if (receita == null)
                return NotFound("Receita não encontrada.");

            return Ok(new
            {
                receita.IdReceita,
                receita.Titulo,
                receita.Imagem,
                receita.Custo,
                receita.Porcao,
                receita.ModoPreparo,
                receita.Status,
                receita.FeedbackChefe,

                Ingredientes = receita.IngredientesReceita.Select(ir => new
                {
                    ir.Ingrediente.IdIngrediente,
                    ir.Ingrediente.NomeIngrediente,
                    ir.Quantidade,
                    ir.Unidade
                })
            });
        }

        // Aprovar Receita
        [HttpPut("aprovar-final/{id}")]
        public IActionResult AprovarFinal(int id)
        {
            var cookie = Request.Cookies["IdLogado"];

            if (cookie == null)
                return Unauthorized();

            var idGestor = Convert.ToInt32(cookie);

            var receita = _context.receita.Find(id);

            if (receita == null)
                return NotFound();

            receita.Status = "Aprovada";

            receita.FkUsuarioGestor = idGestor;

            _context.SaveChanges();

            return Ok();
        }

        // REPROVAR RECEITA
        [HttpPut("reprovar-final/{id}")]
        public IActionResult ReprovarFinal(int id)
        {
            var cookie = Request.Cookies["IdLogado"];

            if (cookie == null)
                return Unauthorized();

            var idGestor = Convert.ToInt32(cookie);

            var receita = _context.receita.Find(id);

            if (receita == null)
                return NotFound();

            receita.Status = "Reprovada";
            receita.FkUsuarioGestor = idGestor;

            _context.SaveChanges();

            return Ok();
        }

        // FEEDBACK DO CHEFE
        [HttpPut("feedback/{id}")]
        public IActionResult FeedbackReceita(int id, [FromBody] FeedbackDTO dto)
        {
            var cookie = Request.Cookies["IdLogado"];

            if (cookie == null)
                return Unauthorized();

            var idChefe = Convert.ToInt32(cookie);

            var receita = _context.receita.FirstOrDefault(r => r.IdReceita == id);

            if (receita == null)
                return NotFound("Receita não encontrada.");

            receita.FeedbackChefe = dto.FeedbackChefe;

            receita.FkUsuarioChefe = idChefe;

            receita.Status = "Gestor";

            _context.SaveChanges();

            return Ok(new
            {
                mensagem = "Feedback enviado para o gestor com sucesso!"
            });
        }
    }
}