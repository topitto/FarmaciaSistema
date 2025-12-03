using FarmaciaSistema.API.Data;
using FarmaciaSistema.Application.Contracts;
using FarmaciaSistema.Domain;
using Microsoft.EntityFrameworkCore;

namespace FarmaciaSistema.API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly FarmaciaSistemaDbContext _context;

        public UsuarioRepository(FarmaciaSistemaDbContext context)
        {
            _context = context;
        }

        public async Task<List<Usuario>> GetAllUsuariosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }

        public async Task AddUsuarioAsync(Usuario usuario)
        {
            await _context.Usuarios.AddAsync(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUsuarioAsync(Usuario usuario)
        {
            // Lógica especial: Solo actualizamos la contraseña si viene una nueva
            var usuarioExistente = await _context.Usuarios.FindAsync(usuario.Id);
            if (usuarioExistente != null)
            {
                usuarioExistente.NombreUsuario = usuario.NombreUsuario;
                usuarioExistente.Rol = usuario.Rol;

                // Si la contraseña no está vacía, la actualizamos. Si está vacía, dejamos la vieja.
                if (!string.IsNullOrEmpty(usuario.PasswordHash))
                {
                    usuarioExistente.PasswordHash = usuario.PasswordHash;
                }

                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }
    }
}