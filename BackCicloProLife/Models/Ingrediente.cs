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

        public virtual ICollection<IngredienteReceita> IngredientesReceita { get; set; }
            = new List<IngredienteReceita>();
    }
}