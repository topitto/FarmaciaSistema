using FarmaciaSistema.API.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using FarmaciaSistema.Application.DTOs;

namespace FarmaciaSistema.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly FarmaciaSistemaDbContext _context;

        public UsuariosController(FarmaciaSistemaDbContext context)
        {
            _context = context;
        }

        // GET: api/Usuarios/Nombres
        [HttpGet("Nombres")]
        public async Task<IActionResult> GetNombresDeUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Select(u => new { u.Id, u.NombreUsuario }) // Seleccionamos solo lo que necesitamos
                .ToListAsync();

            return Ok(usuarios);
        }

        // POST: api/Usuarios/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.NombreUsuario) || string.IsNullOrEmpty(loginRequest.Password))
            {
                return BadRequest("El nombre de usuario y la contraseña son requeridos.");
            }

            // 1. Busca el usuario en la base de datos.
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == loginRequest.NombreUsuario);

            if (usuario == null)
            {
                // Por seguridad, no decimos si falló el usuario o la contraseña.
                return Unauthorized("Credenciales inválidas.");
            }

            // 2. Compara la contraseña.
            // ADVERTENCIA: Esta comparación es insegura para un proyecto real,
            // pero es suficiente para un proyecto escolar.
            if (usuario.PasswordHash != loginRequest.Password)
            {
                return Unauthorized("Credenciales inválidas.");
            }

            // 3. Si todo es correcto, devuelve una respuesta exitosa.
            return Ok(new { Mensaje = "Inicio de sesión exitoso" });
        }
    }
}
