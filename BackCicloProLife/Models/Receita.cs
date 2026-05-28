using System.Security.Principal;

namespace BackCicloProLife.Models
{
    public class Receita
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public double Custo { get; set; }
        public string ModoPreparo { get; set; }
        public double Porcao { get; set; }
        public string Status { get; set; }
        public string FeedbackChefe { get; set; }
        public int IdUsuarioColaborador { get; set; }
        public int IdUsuarioChefe { get; set; }
        public int IdUsuarioGestor { get; set; }

    }
}
