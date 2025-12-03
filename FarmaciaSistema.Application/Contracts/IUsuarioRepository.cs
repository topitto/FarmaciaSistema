using FarmaciaSistema.Domain;

namespace FarmaciaSistema.Application.Contracts
{
    public interface IUsuarioRepository
    {
        Task<List<Usuario>> GetAllUsuariosAsync();
        Task<Usuario> GetUsuarioByIdAsync(int id);
        Task AddUsuarioAsync(Usuario usuario);
        Task UpdateUsuarioAsync(Usuario usuario);
        Task DeleteUsuarioAsync(int id);
    }
}
