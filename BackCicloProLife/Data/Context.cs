using Microsoft.EntityFrameworkCore;
using BackCicloProLife.Models;

namespace BackCicloProLife.Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options){ }

        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Receita> Receita { get; set; }
        public DbSet<Venda> Venda { get; set; }
        public DbSet<Ingrediente> Ingrediente { get; set; }
        public DbSet<IngredienteReceita> IngredienteReceita { get; set; }
    }
}