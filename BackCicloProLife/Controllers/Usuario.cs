using BackCicloProLife.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace BackCicloProLife.Controllers
{
    public class Usuario
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
            [HttpPost("login")]
            public async Task<IActionResult> Login(string email, string senha)
            {
                var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Email == email && u.Senha == senha); //confere os dados do usuario com o banco de dados
                if (usuario == null)
                {
                    return Unauthorized("Email ou senha inválidos.");
                }
                var claims = new List<Claim> // cria as claims para o usuário logado, como nome, email e cargo
                {
                    new Claim(ClaimTypes.Name, usuario.Nome),
                    new Claim(ClaimTypes.Email, usuario.Email),
                    new Claim(ClaimTypes.Role, usuario.Cargo)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); //cria a identidade do usuário / esquema de autenticação de cookies
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
                };
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
                return Ok("Login bem-sucedido.");
            }

            public class UsuariosController : ControllerBase //controlador para gerenciar os usuários, como listar, consultar, cadastrar e deletar
            {
                private readonly Context _context;

                public UsuariosController(Context context)
                {
                    _context = context;
                }

                public ActionResult Index() //método para listar todos os usuários
                {
                    var usuarios = _context.Usuario.ToList();

                    return Ok(usuarios);
                }



                [HttpGet("{id}")] //método para consultar um usuário específico pelo id
                public IActionResult ConsultarUsuario(int id)
                {

                    var usuario = _context.Usuario.Find(id);
                    if (usuario == null)
                    {
                        return NotFound("Usuario não encontrado");
                    }
                    return Ok(usuario);
                }

                //Cadastro do Usuario
                [HttpPost("cadastrar")]
                public IActionResult CadastrarUsuario(Models.Usuario usuario)
                {
                    _context.Usuario.Add(usuario);
                    _context.SaveChanges();
                    return Ok("Usuario cadastrado com sucesso");
                }


                //Deletar Usuario
                [HttpDelete("{id}")]
                public IActionResult DeletarUsuario(int id)
                {
                    var usuario = _context.Usuario.Find(id);
                    if (usuario == null)
                    {
                        return NotFound("Usuario não encontrado");
                    }
                    _context.Usuario.Remove(usuario);
                    _context.SaveChanges();
                    return Ok("Usuario deletado com sucesso");
                }
            }
        }
    }
}
