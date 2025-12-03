using FarmaciaSistema.API.Data;
using FarmaciaSistema.Application.Contracts;
using FarmaciaSistema.Domain;

namespace FarmaciaSistema.API.Repositories
{
    public class CitaRepository : ICitaRepository
    {
        private readonly FarmaciaSistemaDbContext _context;

        public CitaRepository(FarmaciaSistemaDbContext context)
        {
            _context = context;
        }

        public async Task AddCitaAsync(Cita cita)
        {
            await _context.Citas.AddAsync(cita);
            await _context.SaveChangesAsync();
        }
    }
}
