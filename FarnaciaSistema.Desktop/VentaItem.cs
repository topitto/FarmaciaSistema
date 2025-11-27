using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSistema.Desktop
{
    public class VentaItem
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }

        // Calcula el subtotal automáticamente (Precio * Cantidad)
        public decimal Subtotal => PrecioUnitario * Cantidad;
    }
}
