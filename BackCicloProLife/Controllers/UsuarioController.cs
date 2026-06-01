using BackCicloProLife.Data;
using BackCicloProLife.Models;
using Microsoft.AspNetCore.Mvc;

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

        // LOGOUT
        [HttpGet("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session");
            Response.Cookies.Delete("IdLogado");
            return Ok(new { mensagem = "Logout realizado com sucesso!", sucesso = true });
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

            HttpContext.Session.SetString("EmailLogin", resultado[0].Email);
            HttpContext.Session.SetString("IdLogado", resultado[0].IdUsuario.ToString());

            Response.Cookies.Append("IdLogado", resultado[0].IdUsuario.ToString(),
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None
                });

            return Ok(new { mensagem = resultado[0].Nome, cargo = resultado[0].Cargo, sucesso = true });
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
    }

    // para fazer o login sem nome e cargo até irmos para o front
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Senha { get; set; }
    }
}