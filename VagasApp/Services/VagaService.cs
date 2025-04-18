using Microsoft.EntityFrameworkCore;
using VagasApp.Data;
using VagasApp.Models;

namespace VagasApp.Services
{
    public class VagaService
    {
        private readonly VagaContext _context;

        public VagaService(VagaContext context)
        {
            _context = context;
        }

        public async Task<List<Vaga>> ObterVagasAsync()
        {
            return await _context.Vagas.ToListAsync();
        }

        public async Task AtualizarVagaAsync(Vaga vaga)
        {
            _context.Vagas.Update(vaga);
            await _context.SaveChangesAsync();
        }
    }
}
