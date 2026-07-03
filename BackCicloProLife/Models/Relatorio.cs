using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackCicloProLife.Models
{
    [Table("relatorio")]
    public class Relatorio
    {
        [Key]
        [Column("idRelatorio")]
        public int IdRelatorio { get; set; }

        [Column("nomeArquivo")]
        public string NomeArquivo { get; set; }

        [Column("caminhoArquivo")]
        public string CaminhoArquivo { get; set; }

        [Column("dataUpload")]
        public DateTime DataUpload { get; set; }
    }
}