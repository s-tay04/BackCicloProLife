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
        private readonly Context _context; 

        public IngredienteController(Context context)
        {
            _context = context;
        }

        //Cadastrar ingrediente
        [HttpPost("cadastrar")]
        public IActionResult CadastrarIngrediente([FromBody] Ingrediente ingrediente)
        {
            var cookie = Request.Cookies["IdLogado"];

            if (cookie == null)
            {
                return Unauthorized("Realize login para continuar.");
            }

            if (string.IsNullOrWhiteSpace(ingrediente.NomeIngrediente))
            {
                return BadRequest("O nome do ingrediente é obrigatório.");
            }

            var existe = _context.ingrediente.Any(i =>
                i.NomeIngrediente.ToLower().Trim() ==
                ingrediente.NomeIngrediente.ToLower().Trim());

            if (existe)
            {
                return BadRequest("Esse ingrediente já está cadastrado.");
            }

            _context.ingrediente.Add(ingrediente);
            _context.SaveChanges();

            return Created("", ingrediente);
        }

        //Buscar ingredientes
        [HttpGet("buscar")]
        public IActionResult BuscarIngredientes([FromQuery] string? nome)
        {
            var cookie = Request.Cookies["IdLogado"];

            if (cookie == null)
            {
                return Unauthorized("Realize login para continuar.");
            }

            var query = _context.ingrediente.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
            {
                query = query.Where(i =>
                    i.NomeIngrediente.Contains(nome));
            }

            var resultado = query
                .OrderBy(i => i.NomeIngrediente)
                .Select(i => new
                {
                    i.IdIngrediente,
                    i.NomeIngrediente
                })
                .ToList();

            return Ok(resultado);
        }

        // Atualizar ingrediente
        [HttpPut("atualizar")]
        public IActionResult AtualizarIngrediente(int id, Ingrediente ingrediente)
        {
            var cookie = Request.Cookies["IdLogado"];

            if (cookie == null)
            {
                return Unauthorized("Usuário não logado.");
            }

            var ingredienteDoBanco = _context.ingrediente.Find(id);

            if (ingredienteDoBanco == null)
            {
                return NotFound("Ingrediente não encontrado.");
            }

            if (string.IsNullOrWhiteSpace(ingrediente.NomeIngrediente))
            {
                return BadRequest("O nome do ingrediente é obrigatório.");
            }

            ingredienteDoBanco.NomeIngrediente = ingrediente.NomeIngrediente;

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
