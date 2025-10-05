using FarmaciaSistema.Domain; // ¡Importante! Para poder usar tus entidades
using Microsoft.EntityFrameworkCore;

namespace FarmaciaSistema.API.Data
{
    public class FarmaciaSistemaDbContext : DbContext
    {
        public FarmaciaSistemaDbContext(DbContextOptions<FarmaciaSistemaDbContext> options) : base(options)
        {
        }

        // Define un DbSet por cada entidad que quieras que se convierta en tabla
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<CompraDetalle> ComprasDetalles { get; set; }
    }
}
