using FarmaciaSistema.Application.Contracts;
using FarmaciaSistema.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FarmaciaSistema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CitasController : ControllerBase
    {
        private readonly ICitaRepository _citaRepository;

        public CitasController(ICitaRepository citaRepository)
        {
            _citaRepository = citaRepository;
        }

        [HttpPost]
        public async Task<ActionResult> CreateCita(Cita cita)
        {
            cita.Fecha = DateTime.Now; // La fecha se asigna automáticamente al momento de crearla
            await _citaRepository.AddCitaAsync(cita);
            return Ok();
        }
    }
}
