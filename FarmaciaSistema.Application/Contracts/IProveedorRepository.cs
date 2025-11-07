using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FarmaciaSistema.Domain;

namespace FarmaciaSistema.Application.Contracts
{
    public interface IProveedorRepository
    {
        Task<List<Proveedor>> GetAllProveedoresAsync();

        Task<Proveedor> GetProveedorByIdAsync(int id);
        Task AddProveedorAsync(Proveedor proveedor);
        Task UpdateProveedorAsync(Proveedor proveedor);
        Task DeleteProveedorAsync(int id);
    }
}
