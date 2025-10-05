// Dentro de FarmaciaSistema.API/Controllers/ProveedoresController.cs

using FarmaciaSistema.API.Data; // Para usar el DbContext
using FarmaciaSistema.Domain; // Para usar la entidad Proveedor
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaSistema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // La URL será -> /api/Proveedores
    public class ProveedoresController : ControllerBase
    {
        private readonly FarmaciaSistemaDbContext _context;

        // El DbContext se "inyecta" automáticamente aquí gracias a la configuración en Program.cs
        public ProveedoresController(FarmaciaSistemaDbContext context)
        {
            _context = context;
        }

        // Este método manejará las peticiones GET a /api/Proveedores
        [HttpGet]
        public async Task<ActionResult<List<Proveedor>>> GetProveedores()
        {
            // Usa el DbContext para ir a la tabla Proveedores y traerlos todos
            var proveedores = await _context.Proveedores.ToListAsync();

            // Devuelve la lista con un código de estado 200 OK
            return Ok(proveedores);
        }
    }
}
