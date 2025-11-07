// En FarmaciaSistema.API/Controllers/ProductosController.cs
using FarmaciaSistema.API.Repositories;
using FarmaciaSistema.Application.Contracts;
using FarmaciaSistema.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FarmaciaSistema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoRepository _productoRepository;

        public ProductosController(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        // GET: api/Productos
        [HttpGet]
        public async Task<ActionResult<List<Producto>>> GetProductos()
        {
            return Ok(await _productoRepository.GetAllProductosAsync());
        }

        // GET: api/Productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _productoRepository.GetProductoByIdAsync(id);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        // POST: api/Productos
        [HttpPost]
        public async Task<ActionResult> CreateProducto(Producto producto)
        {
            await _productoRepository.AddProductoAsync(producto);
            return CreatedAtAction(nameof(GetProducto), new { id = producto.Id }, producto);
        }

        // PUT: api/Productos/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProducto(int id, Producto producto)
        {
            if (id != producto.Id) return BadRequest();
            await _productoRepository.UpdateProductoAsync(producto);
            return NoContent();
        }

        // DELETE: api/Productos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProducto(int id)
        {
            var producto = await _productoRepository.GetProductoByIdAsync(id);
            if (producto == null) return NotFound();
            await _productoRepository.DeleteProductoAsync(id);
            return NoContent();
        }
    }
}
