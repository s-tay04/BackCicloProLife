using System.Security.Principal;

namespace BackCicloProLife.Models
{
    public class Receita
    {
        public int IdReceita { get; set; }
        public string? Titulo { get; set; }
        public decimal Custo { get; set; }
        public string? ModoPreparo { get; set; }
        public decimal Porcao { get; set; }
        public string? Status { get; set; }
        public string? FeedbackChefe { get; set; }
        public int? FkUsuarioIdUsuarioColaborador { get; set; }
        public int? FkUsuarioIdUsuarioChefe { get; set; }
        public int? FkUsuarioIdUsuarioGestor { get; set; }

    }
}
