using FarmaciaSistema.Application.Contracts;
using FarmaciaSistema.Application.DTOs;
using FarmaciaSistema.Domain;
using Microsoft.AspNetCore.Mvc;

namespace FarmaciaSistema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;

        // Inyectamos el Repositorio, ya no el DbContext directo
        public UsuariosController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        // --- MÉTODOS EXISTENTES (LOGIN Y NOMBRES) ---

        [HttpGet("Nombres")]
        public async Task<IActionResult> GetNombresDeUsuarios()
        {
            // Reutilizamos el GetAll pero seleccionamos solo nombres
            var usuarios = await _usuarioRepository.GetAllUsuariosAsync();
            var nombres = usuarios.Select(u => new { u.Id, u.NombreUsuario }).ToList();
            return Ok(nombres);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            // Nota: Para login rápido, usaremos GetAll y filtraremos en memoria 
            // (en un sistema real, el repositorio debería tener un método GetByUsername)
            var usuarios = await _usuarioRepository.GetAllUsuariosAsync();
            var usuario = usuarios.FirstOrDefault(u => u.NombreUsuario == loginRequest.NombreUsuario);

            if (usuario == null || usuario.PasswordHash != loginRequest.Password)
            {
                return Unauthorized("Credenciales inválidas.");
            }
            return Ok(new { Mensaje = "Inicio de sesión exitoso", Rol = usuario.Rol });
        }

        // --- NUEVOS MÉTODOS CRUD ---

        [HttpGet]
        public async Task<ActionResult<List<Usuario>>> GetUsuarios()
        {
            return Ok(await _usuarioRepository.GetAllUsuariosAsync());
        }

        [HttpPost]
        public async Task<ActionResult> CreateUsuario(Usuario usuario)
        {
            await _usuarioRepository.AddUsuarioAsync(usuario);
            return Ok(usuario);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id) return BadRequest();
            await _usuarioRepository.UpdateUsuarioAsync(usuario);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUsuario(int id)
        {
            await _usuarioRepository.DeleteUsuarioAsync(id);
            return NoContent();
        }
    }
}
