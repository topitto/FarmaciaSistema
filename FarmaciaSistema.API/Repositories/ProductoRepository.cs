// En FarmaciaSistema.API/Repositories/ProductoRepository.cs
using FarmaciaSistema.Application.Contracts;
using FarmaciaSistema.API.Data;
using FarmaciaSistema.Domain;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaSistema.API.Repositories
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly FarmaciaSistemaDbContext _context;

        public ProductoRepository(FarmaciaSistemaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> GetAllProductosAsync()
        {
            return await _context.Productos.ToListAsync();
        }

        public async Task<Producto> GetProductoByIdAsync(int id)
        {
            return await _context.Productos.FindAsync(id);
        }

        public async Task AddProductoAsync(Producto producto)
        {
            await _context.Productos.AddAsync(producto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductoAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductoAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
        }
    }
}