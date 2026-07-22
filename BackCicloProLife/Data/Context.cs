using Microsoft.EntityFrameworkCore;
using BackCicloProLife.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackCicloProLife.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options){ }
        public DbSet<Usuario> usuario { get; set; }
        public DbSet<Receita> receita { get; set; }
        public DbSet<Ingrediente> ingrediente { get; set; }
        public DbSet<IngredienteReceita> ingredienteReceita { get; set; }
        public DbSet<Relatorio> relatorio { get; set; }
    }
}