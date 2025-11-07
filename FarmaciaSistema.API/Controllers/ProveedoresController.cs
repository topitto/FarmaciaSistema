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

        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedor>> GetProveedor(int id)
        {
            var proveedor = await _proveedorRepository.GetProveedorByIdAsync(id);
            if (proveedor == null)
            {
                return NotFound(); // Responde 404 si no se encuentra
            }
            return Ok(proveedor);
        }

        // POST: api/Proveedores
        [HttpPost]
        public async Task<ActionResult> CreateProveedor(Proveedor proveedor)
        {
            await _proveedorRepository.AddProveedorAsync(proveedor);
            // Devuelve 201 Created con la info del nuevo proveedor
            return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.Id }, proveedor);
        }

        // PUT: api/Proveedores/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateProveedor(int id, Proveedor proveedor)
        {
            if (id != proveedor.Id)
            {
                return BadRequest("El Id del proveedor no coincide.");
            }

            await _proveedorRepository.UpdateProveedorAsync(proveedor);
            return NoContent(); // Responde 204 No Content (éxito sin devolver datos)
        }

        // DELETE: api/Proveedores/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProveedor(int id)
        {
            var proveedor = await _proveedorRepository.GetProveedorByIdAsync(id);
            if (proveedor == null)
            {
                return NotFound();
            }

            await _proveedorRepository.DeleteProveedorAsync(id);
            return NoContent(); // Responde 204 No Content
        }
    }
}
