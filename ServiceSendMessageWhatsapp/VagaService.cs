using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServiceSendMessageWhatsapp;

namespace ServiceSendMessageWhatsapp
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

        public async Task GravarPerguntasPendentesAsync(IEnumerable<string> perguntas, string origemVaga)
        {
            foreach (var pergunta in perguntas.Distinct())
            {
                bool existe = await _context.PerguntaPendentes.AnyAsync(p => p.Texto == pergunta);
                if (!existe)
                {
                    _context.PerguntaPendentes.Add(new PerguntaPendente
                    {
                        Texto = pergunta,
                        OrigemVaga = origemVaga
                    });
                }
            }

            await _context.SaveChangesAsync();
        }



    }
}