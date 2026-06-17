using System;
using System.ComponentModel.DataAnnotations;

namespace BackCicloProLife.Models
{
    public class Relatorio
    {
        [Key]
        public int IdRelatorio { get; set; }
        public string NomeArquivo { get; set; } = string.Empty;
        public string CaminhoArquivo { get; set; } = string.Empty;
        public DateTime DataUpload { get; set; }
    }
}