using Microsoft.AspNetCore.Http;

namespace BackCicloProLife.DTOs
{
    public class ReceitaCadastroDTO
    {
        public string Titulo { get; set; } = string.Empty;

        public decimal Custo { get; set; }

        public string ModoPreparo { get; set; } = string.Empty;

        public decimal Porcao { get; set; }

        public IFormFile? ArquivoImagem { get; set; }

        public string Ingredientes { get; set; } = string.Empty;

    }
}
