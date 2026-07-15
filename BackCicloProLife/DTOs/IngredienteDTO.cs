namespace BackCicloProLife.DTOs
{
    public class IngredienteDTO
    {
        public int IdIngrediente { get; set; }

        public decimal Quantidade { get; set; }

        public string Unidade { get; set; } = string.Empty;
    }
}