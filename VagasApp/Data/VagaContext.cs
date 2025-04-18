using Microsoft.EntityFrameworkCore;
using VagasApp.Models;

namespace VagasApp.Data
{
    public class VagaContext : DbContext
    {
        public VagaContext(DbContextOptions<VagaContext> options) : base(options) { }

        public DbSet<Vaga> Vagas { get; set; }
    }
}
