// Dentro de FarmaciaSistema.API/Controllers/ProveedoresController.cs

using FarmaciaSistema.API.Data; // Para usar el DbContext
using FarmaciaSistema.Domain; // Para usar la entidad Proveedor
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FarmaciaSistema.Application.Contracts;

namespace FarmaciaSistema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // La URL será -> /api/Proveedores
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorRepository _proveedorRepository;

        // El DbContext se "inyecta" automáticamente aquí gracias a la configuración en Program.cs
        public ProveedoresController(IProveedorRepository proveedorRepository)
        {
            _proveedorRepository = proveedorRepository;
        }

        // Este método manejará las peticiones GET a /api/Proveedores
        [HttpGet]
        public async Task<ActionResult<List<Proveedor>>> GetProveedores()
        {
            // Ahora llamamos al método del repositorio
            var proveedores = await _proveedorRepository.GetAllProveedoresAsync();
            return Ok(proveedores);
        }
    }
}
