using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FarmaciaSistema.Domain
{
    public class CompraDetalle
    {
        public int Id { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioCompra { get; set; } // El costo al que compramos el producto

        // --- Relaciones ---
        public int CompraId { get; set; } // Llave foránea para la Compra
        public Compra Compra { get; set; }

        public int ProductoId { get; set; } // Llave foránea para el Producto
        public Producto Producto { get; set; }
    }
}
