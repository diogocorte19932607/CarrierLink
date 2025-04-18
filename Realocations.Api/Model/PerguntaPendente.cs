namespace Realocations.Api.Model
{
    public class PerguntaPendente
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public string OrigemVaga { get; set; }
        public DateTime DataRegistro { get; set; } = DateTime.Now;

    }
}
