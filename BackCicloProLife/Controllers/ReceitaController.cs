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
        public IActionResult CadastrarReceita(Models.Receita receita)
        {
            // Valida se o Colaborador existe no banco
            if (receita.FkUsuarioColaborador.HasValue && receita.FkUsuarioColaborador.Value > 0)
            {
                var colaboradorExiste = _context.usuario.Any(u => u.IdUsuario == receita.FkUsuarioColaborador);
                if (!colaboradorExiste)
                    return BadRequest($"O ID de Colaborador '{receita.FkUsuarioColaborador}' não existe na tabela de usuários.");
            }

            // Valida se o Chefe existe no banco
            if (receita.FkUsuarioChefe.HasValue && receita.FkUsuarioChefe.Value > 0)
            {
                var chefeExiste = _context.usuario.Any(u => u.IdUsuario == receita.FkUsuarioChefe);
                if (!chefeExiste)
                    return BadRequest($"O ID de Chefe '{receita.FkUsuarioChefe}' não existe na tabela de usuários.");
            }

            // Valida se o Gestor existe no banco
            if (receita.FkUsuarioGestor.HasValue && receita.FkUsuarioGestor.Value > 0)
            {
                var gestorExiste = _context.usuario.Any(u => u.IdUsuario == receita.FkUsuarioGestor);
                if (!gestorExiste)
                    return BadRequest($"O ID de Gestor '{receita.FkUsuarioGestor}' não existe na tabela de usuários.");
            }

            _context.receita.Add(receita);
            _context.SaveChanges();

            return Created("", receita);
        }
    }
}