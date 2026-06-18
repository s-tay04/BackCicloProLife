using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackCicloProLife.Models
{
    [Table("receita")]
    public class Receita
    {
        [Key]
        [Column("idReceita")]
        public int IdReceita { get; set; }

        [Column("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [Column("custo")]
        public decimal Custo { get; set; }

        [Column("modoPreparo")]
        public string ModoPreparo { get; set; } = string.Empty;

        [Column("porcao")]
        public decimal Porcao { get; set; }

        [Column("status")]
        public string Status { get; set; } = string.Empty;

        [Column("dataCadastro")]
        public DateTime DataCadastro { get; set; }

        [Column("feedbackChefe")]
        public string FeedbackChefe { get; set; } = string.Empty;

        [Column("fk_usuario_idUsuarioColaborador")]
        public int? FkUsuarioColaborador { get; set; }

        [Column("fk_usuario_idUsuarioChefe")]
        public int? FkUsuarioChefe { get; set; }

        [Column("fk_usuario_idUsuarioGestor")]
        public int? FkUsuarioGestor { get; set; }

        [Column("imagem")]
        public string? Imagem { get; set; }

        [NotMapped]
        public IFormFile? ArquivoImagem { get; set; }
    }
}