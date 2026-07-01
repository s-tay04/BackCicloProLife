using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackCicloProLife.Models
{
    [Table("usuario")]
    public class Usuario
    {
        [Key]
        [Column("idUsuario")]
        public int IdUsuario { get; set; }

        [Required]
        [Column("nome")]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        [Column("email")]
        public string Email { get; set; }

        [Required]
        [Column("senha")]
        public string Senha { get; set; }

        [Required]
        [Column("cargo")]
        public string Cargo { get; set; }
    }
}