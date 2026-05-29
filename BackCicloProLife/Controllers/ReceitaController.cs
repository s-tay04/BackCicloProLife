using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackCicloProLife.Data;
using BackCicloProLife.Models;

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

        [HttpPost("cadastrar")]
        public IActionResult CadastrarReceita(Receita receita)
        {
            var idLogado = Request.Cookies["IdLogado"];

            if (idLogado == null)
            {
                return Unauthorized("Realize o login para continuar.");
            }

            receita.FkUsuarioIdUsuarioColaborador = int.Parse(idLogado);

            var usuarioExiste = _context.usuario.Any(u => u.IdUsuario == receita.FkUsuarioIdUsuarioColaborador);

            if (!usuarioExiste)
            {
                return NotFound($"Erro: O usuário com ID {receita.FkUsuarioIdUsuarioColaborador} não existe.");
            }

            _context.receita.Add(receita);
            _context.SaveChanges();

            return Created("", receita);
        }
    }
}