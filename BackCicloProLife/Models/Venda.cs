using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackCicloProLife.Models
{
    [Table("venda")]
    public class Venda
    {
        [Key]
        [Column("idVenda")]
        public int IdVenda { get; set; }

        [Column("data")]
        public DateOnly Data { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("totalDiario")]
        public decimal TotalDiario { get; set; }

        [Column("fk_receita_idReceita")]
        public int? FkReceita { get; set; }
    }
}