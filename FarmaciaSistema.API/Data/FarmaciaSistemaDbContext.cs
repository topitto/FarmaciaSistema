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

        // Dentro de la clase FarmaciaSistemaDbContext

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // Aquí agregamos los datos iniciales para la entidad Proveedor
            modelBuilder.Entity<Proveedor>().HasData(
                new Proveedor
                {
                    Id = 1,
                    Nombre = "Proveedor Principal",
                    Contacto = "Juan Pérez",
                    Telefono = "6621234567"
                },
                new Proveedor
                {
                    Id = 2,
                    Nombre = "Bayer de México",
                    Contacto = "Ana García",
                    Telefono = "6627654321"
                }
            );
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    NombreUsuario = "admin",
                    // La contraseña es "1234". Esto es solo para pruebas, luego lo haremos más seguro.
                    PasswordHash = "1234",
                    Rol = "Administrador"
                }
            );
        }
    }
}
