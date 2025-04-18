using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceSendMessageWhatsapp
{
    public class PerguntaPendente
    {
        public int Id { get; set; }
        public string Texto { get; set; }
        public string OrigemVaga { get; set; }
        public DateTime DataRegistro { get; set; } = DateTime.Now;

    }
}
