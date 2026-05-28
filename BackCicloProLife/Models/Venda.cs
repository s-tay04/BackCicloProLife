namespace BackCicloProLife.Models
{
    public class Venda
    {
        public int IdVenda { get; set; }
        public DateOnly Data {  get; set; }
        public int Quantidade { get; set; }
        public double TotalDiario { get; set; }
        public int IdReceita { get; set; }
    }
}
