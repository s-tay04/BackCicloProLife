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
    }
}
