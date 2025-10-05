using FarmaciaSistema.Application.Contracts;
using FarmaciaSistema.API.Data;
using FarmaciaSistema.Domain;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaSistema.API.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly FarmaciaSistemaDbContext _context;

        public ProveedorRepository(FarmaciaSistemaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Proveedor>> GetAllProveedoresAsync()
        {
            return await _context.Proveedores.ToListAsync();
        }
    }
}
