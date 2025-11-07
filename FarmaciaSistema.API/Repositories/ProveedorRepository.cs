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

        public async Task<Proveedor> GetProveedorByIdAsync(int id)
        {
            return await _context.Proveedores.FindAsync(id);
        }

        public async Task AddProveedorAsync(Proveedor proveedor)
        {
            await _context.Proveedores.AddAsync(proveedor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProveedorAsync(Proveedor proveedor)
        {
            _context.Proveedores.Update(proveedor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProveedorAsync(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor != null)
            {
                _context.Proveedores.Remove(proveedor);
                await _context.SaveChangesAsync();
            }
        }
    }
}
