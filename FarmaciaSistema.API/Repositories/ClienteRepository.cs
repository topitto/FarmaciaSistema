// En FarmaciaSistema.API/Repositories/ClienteRepository.cs
using FarmaciaSistema.Application.Contracts;
using FarmaciaSistema.API.Data;
using FarmaciaSistema.Domain;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaSistema.API.Repositories
{
    public class ClienteRepository : IClienteRepository
    {
        private readonly FarmaciaSistemaDbContext _context;

        public ClienteRepository(FarmaciaSistemaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> GetAllClientesAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<Cliente> GetClienteByIdAsync(int id)
        {
            return await _context.Clientes.FindAsync(id);
        }

        public async Task AddClienteAsync(Cliente cliente)
        {
            await _context.Clientes.AddAsync(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateClienteAsync(Cliente cliente)
        {
            _context.Clientes.Update(cliente);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteClienteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                _context.Clientes.Remove(cliente);
                await _context.SaveChangesAsync();
            }
        }
    }
}
