using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackCicloProLife.Models
{
    [Table("ingrediente")]
    public class Ingrediente
    {
        [Key]
        [Column("idIngrediente")]
        public int IdIngrediente { get; set; }

        [Column("nomeIngrediente")]
        public string NomeIngrediente { get; set; } = string.Empty;

        [Column("unidadeFornecimento")]
        public string UnidadeFornecimento { get; set; } = string.Empty;
    }
}