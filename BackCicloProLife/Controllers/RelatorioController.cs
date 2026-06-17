using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using BackCicloProLife.Data;
using BackCicloProLife.Models;
using System.ComponentModel;

[ApiController]
[Route("[controller]")]
public class RelatorioController : ControllerBase
{
    private readonly Context _context;

    public RelatorioController(Context context)
    {
        _context = context;
        OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
    }

    [HttpPost("upload")]
    public IActionResult UploadRelatorio(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("Arquivo inválido.");

        using (var stream = new MemoryStream())
        {
            file.CopyTo(stream);
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    // O ?. e o ?? "" garantem que se a célula estiver vazia, ela vira um texto vazio em vez de crashar
                    string nomeReceita = worksheet.Cells[row, 1].Value?.ToString() ?? "";
                    string qtdTexto = worksheet.Cells[row, 2].Value?.ToString() ?? "0";
                    string totalTexto = worksheet.Cells[row, 3].Value?.ToString() ?? "0";

                    int quantidade = int.Parse(qtdTexto);
                    decimal total = decimal.Parse(totalTexto);

                    var receita = _context.receita.FirstOrDefault(r => r.Titulo == nomeReceita);

                    if (receita != null)
                    {
                        var novaVenda = new Venda
                        {
                            Data = DateOnly.FromDateTime(DateTime.Now),
                            Quantidade = quantidade,
                            TotalDiario = total,
                            FkReceita = receita.IdReceita
                        };
                        _context.venda.Add(novaVenda);
                    }
                }
                _context.SaveChanges();
            }
        }
        return Ok("Dados importados com sucesso!");
    }
}