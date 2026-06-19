using BackCicloProLife.Data;
using BackCicloProLife.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace BackCicloProLife.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly Context _context;

        public UsuarioController(Context context)
        {
            _context = context;
        }

        // CADASTRO
        [HttpPost("cadastrar")]
        public IActionResult Cadastrar([FromBody] Usuario usuario)
        {
            var emailExistente = _context.usuario
                .Any(u => u.Email.Equals(usuario.Email));

            if (emailExistente)
            {
                return Conflict(new { mensagem = "Já existe um usuário com esse e-mail.", sucesso = false });
            }

            _context.usuario.Add(usuario);
            _context.SaveChanges();

            return Created("", new { mensagem = "Usuário cadastrado com sucesso!", sucesso = true });
        }

        // LOGIN
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var resultado = _context.usuario
                .Where(u => u.Email.Equals(request.Email) && u.Senha.Equals(request.Senha))
                .ToList();

            if (resultado.Count == 0)
            {
                return Unauthorized(new { mensagem = "Email ou senha inválidos.", sucesso = false });
            }

            Response.Cookies.Append("IdLogado", resultado[0].IdUsuario.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                Path = "/"
            });

            return Ok(new { mensagem = resultado[0].Nome, cargo = resultado[0].Cargo, sucesso = true });
        }

        // VERIFICAR SE ESTÁ LOGADO
        [HttpGet("inicial")]
        public IActionResult Inicial()
        {
            var usuarioSessao = HttpContext.Session.GetString("EmailLogin");
            if (string.IsNullOrEmpty(usuarioSessao))
            {
                return Unauthorized("Usuário não está logado!");
            }
            return Ok(new { mensagem = "Usuário logado", email = usuarioSessao });
        }

        // BUSCAR DADOS DO PERFIL LOGADO
        [HttpGet("perfil")]
        public IActionResult ObterPerfil()
        {
            // Lê o cookie puro que injetamos acima
            var idCookie = Request.Cookies["IdLogado"];

            if (string.IsNullOrEmpty(idCookie))
            {
                return Unauthorized(new { mensagem = "Usuário não está logado!" });
            }

            int idUsuario = int.Parse(idCookie);
            var usuarioBanco = _context.usuario.Find(idUsuario);

            if (usuarioBanco == null)
            {
                return NotFound(new { mensagem = "Usuário não encontrado." });
            }

            return Ok(new
            {
                nome = usuarioBanco.Nome,
                email = usuarioBanco.Email,
                cargo = usuarioBanco.Cargo
            });
        }

        // LOGOUT
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("IdLogado", new CookieOptions
            {
                Secure = false,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });
            return Ok(new { mensagem = "Logout realizado com sucesso!", sucesso = true });
        }

        // DELETAR
        [HttpDelete("delete/{id}")]
        public IActionResult DeletarUsuario(int id)
        {
            var sessao = HttpContext.Session.GetString("IdLogado");
            if (sessao == null)
            {
                return Unauthorized("Realize o login para continuar.");
            }

            var usuarioBanco = _context.usuario.Find(id);

            if (usuarioBanco == null)
                return NotFound("Usuário não encontrado.");

            _context.usuario.Remove(usuarioBanco);
            _context.SaveChanges();

            return Ok("Usuário deletado!");
        }
    }

    // Para fazer o login sem nome e cargo até irmos para o front
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
    }
}