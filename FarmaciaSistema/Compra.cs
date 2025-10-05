using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSistema.Domain
{
    public class Compra
    {
        public int Id { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }

        // --- Relaciones ---
        public int ProveedorId { get; set; } // Llave foránea para el Proveedor
        public Proveedor Proveedor { get; set; } // Propiedad de navegación

        // Una compra tiene muchos detalles (productos)
        public List<CompraDetalle> Detalles { get; set; }
    }
}
