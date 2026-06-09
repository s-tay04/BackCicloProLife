using Microsoft.AspNetCore.Mvc;
using BackCicloProLife.Data;
using BackCicloProLife.Models;
using Microsoft.AspNetCore.Http;

namespace BackCicloProLife.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IngredienteController : ControllerBase
    {
        private readonly Context _context; //liga ao banco de dados

        public IngredienteController(Context context)
        {
            _context = context;
        }

        //Cadastrar ingrediente
        [HttpPost("cadastrar")]
        public IActionResult CadastrarIngrediente(Ingrediente ingrediente)
        {
            if (string.IsNullOrWhiteSpace(ingrediente.NomeIngrediente))
            {
                return BadRequest("O nome do ingrediente é obrigatório."); //Verifica se o espaço está preenchido
            }

            if (string.IsNullOrWhiteSpace(ingrediente.UnidadeFornecimento))
            {
                return BadRequest("A unidade de fornecimento é obrigatória.");
            }

            _context.ingrediente.Add(ingrediente);
            _context.SaveChanges();

            return Created("", ingrediente);
        }

        //Buscar ingredientes
        [HttpGet("buscar")]
        public IActionResult BuscarIngredientes([FromQuery] string? nome) //permite que o usuario busque o ingrediente pelo nome
        {
            var query = _context.ingrediente.AsQueryable();

            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(i => i.NomeIngrediente.Contains(nome));
            }
            var resultado = query.ToList();
            return Ok(resultado);
        }

        //Atualizar ingrediente
        [HttpPut("atualizar")]
        public IActionResult AtualizarIngrediente(int id, Ingrediente ingrediente) //Permite atulizar (só é possivel quando o usuario esta logado)
        {
            var sessao = HttpContext.Session.GetString("IdLogado");

            if (sessao == null)
            {
                return Unauthorized("Usuário não logado."); 
            }

            var igredienteDoBanco = _context.ingrediente.Find(id); //procura o ingrediente pelo id
            if (igredienteDoBanco == null)
            {
                return NotFound("Ingrediente não encontrado.");
            }

            _context.SaveChanges();
            return Ok(igredienteDoBanco); //salva a atualização do ingrediente no banco de dados
        }
    }
}
