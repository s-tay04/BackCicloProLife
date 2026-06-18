using Microsoft.AspNetCore.Mvc;
using BackCicloProLife.Data;
using BackCicloProLife.Models;
using Microsoft.AspNetCore.Http;

namespace BackCicloProLife.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IngredienteReceitaController: ControllerBase
    {
        private readonly Context _context;
        public IngredienteReceitaController(Context context)
        {
            _context = context;
        }

        //cadastrar ingredientes em receita 
        [HttpPost("cadastrar")]
        public IActionResult CadastrarIngredienteReceita(IngredienteReceita ingredienteReceita)
        {
            var sessao = HttpContext.Session.GetInt32("IdLogado");

            if (sessao == null)
            {
                return BadRequest("Realize o login para prosseguir");
            }

            //validar se o ingrediente existe
            var ingredienteExiste = _context.ingrediente.Any(i => i.IdIngrediente == ingredienteReceita.FkIngrediente);

            if (!ingredienteExiste)
            {
                return BadRequest("O ingrediente informado não existe.");
            }

            var receitaExiste = _context.receita.Any(r => r.IdReceita == ingredienteReceita.FkReceita);
            if (!receitaExiste)
            {
                return BadRequest("A receita informada não existe.");
            }

            if (ingredienteReceita.Quantidade <= 0)
            {
                return BadRequest("A quantidade deve ser maior que zero.");

            }

            var Exise = _context.ingredienteReceita.Any(ir => ir.FkIngrediente == ingredienteReceita.FkIngrediente && ir.FkReceita == ingredienteReceita.FkReceita);

            if (Exise)
            {
                return BadRequest("Esse ingrediente já está cadastrado nessa receita.");
            }

            _context.ingredienteReceita.Add(ingredienteReceita); //após todas as verificações, o ingrediente é adicionado à receita
            _context.SaveChanges();

            return Created("", ingredienteReceita);

        }
        // Buscar ingredientes de uma receita
        [HttpGet("buscar")]
        public IActionResult BuscarIngredienteReceita([FromQuery] int? idReceita)
        {
            var sessao = HttpContext.Session.GetInt32("IdLogado");

            if (sessao == null)
            {
                return BadRequest("Realize o login para prosseguir"); 
            }


            var query = _context.ingredienteReceita.AsQueryable(); //filtra os ingredientes de uma receita específica, caso o id da receita seja fornecido

            if (idReceita.HasValue)
            {
                query = query.Where(ir => ir.FkReceita == idReceita);
            }

            var resultado = query.ToList();

            return Ok(resultado);
        }

        // Atualizar quantidade do ingrediente na receita
        [HttpPut("atualizar")]
        public IActionResult AtualizarIngredienteReceita(int fkIngrediente, int fkReceita, IngredienteReceita ingredienteReceita)
        {
            var sessao = HttpContext.Session.GetInt32("IdLogado");

            if (sessao == null)
            {
                return BadRequest("Realize o login para prosseguir");
            }

            var ingredienteReceitaDoBanco = _context.ingredienteReceita.Find(fkIngrediente, fkReceita);

            if (ingredienteReceitaDoBanco == null)
            {
                return NotFound("Ingrediente não encontrado nessa receita.");
            }

            if (ingredienteReceita.Quantidade <= 0)
            {
                return BadRequest("A quantidade deve ser maior que zero.");
            }

            ingredienteReceitaDoBanco.Quantidade = ingredienteReceita.Quantidade; //verifica se a quantidade é válida e, se for, atualiza a quantidade do ingrediente na receita

            _context.SaveChanges(); //salva as alterações no banco de dados

            return Ok("Quantidade atualizada com sucesso.");
        }

        // Deletar ingrediente da receita
        [HttpDelete("deletar")]
        public IActionResult DeletarIngredienteReceita(int fkIngrediente, int fkReceita)
        {
            var sessao = HttpContext.Session.GetInt32("IdLogado");

            if (sessao == null)
            {
                return BadRequest("Realize o login para prosseguir");
            }

            var ingredienteReceita = _context.ingredienteReceita.Find(fkIngrediente, fkReceita);

            if (ingredienteReceita == null)
            {
                return NotFound("Ingrediente não encontrado nessa receita.");
            }

            _context.ingredienteReceita.Remove(ingredienteReceita);
            _context.SaveChanges();

            return Ok("Ingrediente removido da receita com sucesso.");
        }


        

        // COMANDOS PARA TESTAR NO POSTMAN

        // 1 CADASTRAR
        // Método: POST
        // URL: https://localhost:7037/IngredienteReceita/cadastrar
        // Body > raw > JSON:
        // {
        //   "fkIngrediente": 1,
        //   "fkReceita": 1,
        //   "quantidade": 2.5
        // }

        // --------------------------------------------------

        // 2. BUSCAR TODOS
        // Método: GET
        // URL: https://localhost:7037/IngredienteReceita/buscar
        // Body: não precisa

        // --------------------------------------------------

        // 3. BUSCAR POR RECEITA
        // Método: GET
        // URL: https://localhost:7037/IngredienteReceita/buscar?idReceita=1
        // Body: não precisa

        // --------------------------------------------------

        // 4. ATUALIZAR QUANTIDADE
        // Método: PUT
        // URL: https://localhost:7037/IngredienteReceita/atualizar?fkIngrediente=1&fkReceita=1
        // Body > raw > JSON:
        // {
        //   "fkIngrediente": 1,
        //   "fkReceita": 1,
        //   "quantidade": 5
        // }

        // --------------------------------------------------

        // 5. DELETAR
        // Método: DELETE
        // URL: https://localhost:7037/IngredienteReceita/deletar?fkIngrediente=1&fkReceita=1
        // Body: não precisa
    }
}
