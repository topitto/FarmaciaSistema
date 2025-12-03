using FarmaciaSistema.API.Data;
using FarmaciaSistema.Application.DTOs; // Para usar los DTOs que acabamos de crear
using FarmaciaSistema.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaSistema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly FarmaciaSistemaDbContext _context;

        public VentasController(FarmaciaSistemaDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult> CreateVenta(VentaDto ventaDto)
        {
            // Usamos una transacción para que si algo falla, no se guarde nada a medias
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // 1. Crear la Venta Maestra (Cabecera)
                var venta = new Venta
                {
                    Fecha = DateTime.Now,
                    Total = ventaDto.Total,
                    UsuarioId = ventaDto.UsuarioId,
                    Detalles = new List<DetalleVenta>()
                };

                // 2. Procesar cada producto de la lista (Detalles)
                foreach (var item in ventaDto.Detalles)
                {
                    // Buscamos el producto en la BD para ver su stock real
                    var producto = await _context.Productos.FindAsync(item.ProductoId);

                    if (producto == null)
                    {
                        return BadRequest($"El producto con ID {item.ProductoId} no existe.");
                    }

                    // --- VALIDACIÓN DE STOCK ---
                    if (producto.Stock < item.Cantidad)
                    {
                        return BadRequest($"No hay suficiente stock para el producto '{producto.Nombre}'. Stock actual: {producto.Stock}");
                    }

                    // --- RESTA DE INVENTARIO ---
                    producto.Stock -= item.Cantidad;

                    // Agregamos el detalle a la venta
                    venta.Detalles.Add(new DetalleVenta
                    {
                        ProductoId = item.ProductoId,
                        Cantidad = item.Cantidad,
                        PrecioUnitario = item.PrecioUnitario
                    });
                }

                // 3. Guardar todo en la base de datos
                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                // 4. Confirmar la transacción
                await transaction.CommitAsync();

                return Ok(new { Mensaje = "Venta registrada con éxito", VentaId = venta.Id });
            }
            catch (Exception ex)
            {
                // Si algo falló, deshacemos cualquier cambio (rollback)
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error interno al procesar la venta: {ex.Message}");
            }
        }
    }
}
