using Microsoft.EntityFrameworkCore;
using Realocations.Api.Model;
using VagasApp.Models;

namespace VagasApp.Data
{
    public class VagaContext : DbContext
    {
        public VagaContext(DbContextOptions<VagaContext> options) : base(options) { }

        public DbSet<Vaga> Vagas { get; set; }
        public DbSet<PerguntaPendente> PerguntaPendentes { get; set; }
    }
}
