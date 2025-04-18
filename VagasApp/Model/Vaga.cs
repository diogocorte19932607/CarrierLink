namespace VagasApp.Models
{
    public class Vaga
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Empresa { get; set; }
        public string Local { get; set; }
        public string Modelo { get; set; }
        public string CandidaturaSimplificada { get; set; }

        // 🔧 Aqui está o ajuste!
        public DateTime Data { get; set; }

        public string Link { get; set; }
        public bool Aplicada { get; set; }
    }
}
