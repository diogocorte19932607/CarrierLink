using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ServiceSendMessageWhatsapp
{

    public class VagaContext : DbContext
    {
        public VagaContext()
        {
        }

        public VagaContext(DbContextOptions<VagaContext> options) : base(options) { }

        public DbSet<Vaga> Vagas { get; set; }
        public DbSet<PerguntaPendente> PerguntaPendentes { get; set; }
    }
}
