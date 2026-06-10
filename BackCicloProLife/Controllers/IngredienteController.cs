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
            var sessao = HttpContext.Session.GetString("IdLogado");
            if (sessao == null)
            {
                return Unauthorized("Realize login para continuar."); //Verifica se o usuario esta logado, caso não esteja, ele não pode cadastrar o ingrediente
            }

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
            var sessao = HttpContext.Session.GetString("IdLogado");

            if (sessao == null)
            {
                return Unauthorized("Realize login para continuar."); //Verifica se o usuario esta logado, caso não esteja, ele não pode deletar o ingrediente
            }

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

            var ingredienteDoBanco = _context.ingrediente.Find(id); //procura o ingrediente pelo id
            if (ingredienteDoBanco == null)
            {
                return NotFound("Ingrediente não encontrado.");
            }

            if (string.IsNullOrWhiteSpace(ingrediente.NomeIngrediente))
            {
                return BadRequest("O nome do ingrediente é obrigatório.");
            }

            if (string.IsNullOrWhiteSpace(ingrediente.UnidadeFornecimento))
            {
                return BadRequest("A unidade de fornecimento é obrigatória.");
            }

            ingredienteDoBanco.NomeIngrediente = ingrediente.NomeIngrediente;
            ingredienteDoBanco.UnidadeFornecimento = ingrediente.UnidadeFornecimento;

            _context.SaveChanges();

            return Ok("Ingrediente atualizado com sucesso.");
        }

        
        //Deletar ingrediente
        [HttpDelete("deletar/{id}")]
        public IActionResult DeletarIngredientes(int id)
        {
            var sessao = HttpContext.Session.GetString("IdLogado");

            if (sessao == null)
            {
                return Unauthorized("Realize login para continuar."); //Verifica se o usuario esta logado, caso não esteja, ele não pode deletar o ingrediente
            }


            var ingrediente = _context.ingrediente.Find(id);

            if (ingrediente == null)
            {
                return NotFound("Ingrediente não encontrado.");
            }

            _context.ingrediente.Remove(ingrediente);
            _context.SaveChanges();

            return Ok("Ingrediente deletado!");
        }
    }
}
