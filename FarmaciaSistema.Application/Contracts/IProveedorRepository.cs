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
    }
}
