// En FarmaciaSistema.API/Controllers/ClientesController.cs
using FarmaciaSistema.Application.Contracts;
using FarmaciaSistema.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FarmaciaSistema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteRepository _clienteRepository;

        public ClientesController(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> GetClientes()
        {
            return Ok(await _clienteRepository.GetAllClientesAsync());
        }

        // GET: api/Clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> GetCliente(int id)
        {
            var cliente = await _clienteRepository.GetClienteByIdAsync(id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<ActionResult> CreateCliente(Cliente cliente)
        {
            await _clienteRepository.AddClienteAsync(cliente);
            return CreatedAtAction(nameof(GetCliente), new { id = cliente.Id }, cliente);
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateCliente(int id, Cliente cliente)
        {
            if (id != cliente.Id) return BadRequest();
            await _clienteRepository.UpdateClienteAsync(cliente);
            return NoContent();
        }

        // DELETE: api/Clientes/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCliente(int id)
        {
            var cliente = await _clienteRepository.GetClienteByIdAsync(id);
            if (cliente == null) return NotFound();
            await _clienteRepository.DeleteClienteAsync(id);
            return NoContent();
        }
    }
}
