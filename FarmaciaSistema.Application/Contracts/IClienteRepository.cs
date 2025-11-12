// En FarmaciaSistema.Application/Contracts/IClienteRepository.cs
using FarmaciaSistema.Domain;

namespace FarmaciaSistema.Application.Contracts
{
    public interface IClienteRepository
    {
        Task<List<Cliente>> GetAllClientesAsync();
        Task<Cliente> GetClienteByIdAsync(int id);
        Task AddClienteAsync(Cliente cliente);
        Task UpdateClienteAsync(Cliente cliente);
        Task DeleteClienteAsync(int id);
    }
}
