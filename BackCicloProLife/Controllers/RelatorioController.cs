using BackCicloProLife.Data;
using BackCicloProLife.Models;
using Microsoft.AspNetCore.Mvc;

namespace BackCicloProLife.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RelatorioController : ControllerBase
    {
        private readonly Context _context;
        private readonly IWebHostEnvironment _env;

        public RelatorioController(Context context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        // UPLOAD
        [HttpPost("upload")]
        public async Task<IActionResult> UploadRelatorio(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("Arquivo inválido.");

                string pasta = Path.Combine(
                    _env.ContentRootPath,
                    "Uploads"
                );

                if (!Directory.Exists(pasta))
                    Directory.CreateDirectory(pasta);

                string nomeArquivo =
                    $"{Guid.NewGuid()}_{file.FileName}";

                string caminhoCompleto =
                    Path.Combine(pasta, nomeArquivo);

                using (var stream =
                    new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                Relatorio relatorio = new Relatorio
                {
                    NomeArquivo = file.FileName,
                    CaminhoArquivo = nomeArquivo,
                    DataUpload = DateTime.Now
                };

                _context.relatorio.Add(relatorio);

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensagem = "Relatório enviado com sucesso!"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // LISTAR
        [HttpGet("listar")]
        public IActionResult ListarRelatorios(
            string ordem = "desc"
        )
        {
            var relatorios = ordem == "asc"
                ? _context.relatorio
                    .OrderBy(r => r.DataUpload)
                    .ToList()
                : _context.relatorio
                    .OrderByDescending(r => r.DataUpload)
                    .ToList();

            return Ok(relatorios);
        }

        // DOWNLOAD
        [HttpGet("download/{id}")]
        public IActionResult DownloadRelatorio(int id)
        {
            var relatorio = _context.relatorio
                .FirstOrDefault(r =>
                    r.IdRelatorio == id
                );

            if (relatorio == null)
                return NotFound();

            string caminho = Path.Combine(
                _env.ContentRootPath,
                "Uploads",
                relatorio.CaminhoArquivo
            );

            var bytes = System.IO.File.ReadAllBytes(caminho);

            return File(
                bytes,
                "application/octet-stream",
                relatorio.NomeArquivo
            );
        }
    }
}