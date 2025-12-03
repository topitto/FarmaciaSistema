using FarmaciaSistema.Domain;
namespace FarmaciaSistema.Application.Contracts
{
    public interface ICitaRepository
    {
        Task AddCitaAsync(Cita cita);
        // Podríamos agregar GetCitasByClienteId más adelante para ver el historial
    }
}
