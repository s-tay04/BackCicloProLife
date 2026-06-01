using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackCicloProLife.Models
{
    [Table("ingredienteReceita")]
    [PrimaryKey(nameof(FkIngrediente), nameof(FkReceita))]
    public class IngredienteReceita
    {
        [Column("fk_ingrediente_idIngrediente")]
        public int FkIngrediente { get; set; }

        [Column("fk_receita_idReceita")]
        public int FkReceita { get; set; }

        [Column("quantidade")]
        public decimal Quantidade { get; set; }
    }
}