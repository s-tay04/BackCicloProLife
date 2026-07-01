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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emailExistente = _context.usuario
                .Any(u => u.Email.ToLower() == usuario.Email.ToLower());

            if (emailExistente)
            {
                return Conflict(new
                {
                    mensagem = "Já existe um usuário com esse e-mail.",
                    sucesso = false
                });
            }

            _context.usuario.Add(usuario);
            _context.SaveChanges();

            return Created("", new
            {
                mensagem = "Usuário cadastrado com sucesso!",
                sucesso = true
            });
        }

        // LOGIN
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var usuario = _context.usuario
                .FirstOrDefault(u =>
                    u.Email.ToLower() == request.Email.ToLower()
                    && u.Senha == request.Senha);

            if (usuario == null)
            {
                return Unauthorized(new
                {
                    mensagem = "Email ou senha inválidos.",
                    sucesso = false
                });
            }

            Response.Cookies.Append("IdLogado", usuario.IdUsuario.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                Path = "/"
            });

            return Ok(new
            {
                mensagem = usuario.Nome,
                cargo = usuario.Cargo,
                sucesso = true
            });
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
            var idCookie = Request.Cookies["IdLogado"];

            if (string.IsNullOrEmpty(idCookie))
            {
                return Unauthorized(new
                {
                    mensagem = "Usuário não está logado!",
                    sucesso = false
                });
            }

            if (!int.TryParse(idCookie, out int idUsuario))
            {
                return BadRequest(new
                {
                    mensagem = "Cookie inválido.",
                    sucesso = false
                });
            }

            var usuarioBanco = _context.usuario.FirstOrDefault(u => u.IdUsuario == idUsuario);

            if (usuarioBanco == null)
            {
                return NotFound(new
                {
                    mensagem = "Usuário não encontrado.",
                    sucesso = false
                });
            }

            return Ok(new
            {
                nome = usuarioBanco.Nome,
                email = usuarioBanco.Email,
                cargo = usuarioBanco.Cargo,
                sucesso = true
            });
        }

        // ALTERAR PERFIL
        [HttpPut("alterarPerfil")]
        public IActionResult AtualizarPerfil([FromBody] Usuario usuarioAtualizado)
        {
            var idCookie = Request.Cookies["IdLogado"];

            if (string.IsNullOrEmpty(idCookie))
            {
                return Unauthorized(new
                {
                    mensagem = "Usuário não está logado!",
                    sucesso = false
                });
            }

            if (!int.TryParse(idCookie, out int idUsuario))
            {
                return BadRequest(new
                {
                    mensagem = "Cookie inválido.",
                    sucesso = false
                });
            }

            var usuarioBanco = _context.usuario
                .FirstOrDefault(u => u.IdUsuario == idUsuario);

            if (usuarioBanco == null)
            {
                return NotFound(new
                {
                    mensagem = "Usuário não encontrado.",
                    sucesso = false
                });
            }

            usuarioBanco.Nome = usuarioAtualizado.Nome;
            usuarioBanco.Email = usuarioAtualizado.Email;
            usuarioBanco.Cargo = usuarioAtualizado.Cargo;

            _context.SaveChanges();

            return Ok(new
            {
                mensagem = "Perfil atualizado com sucesso!",
                sucesso = true
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