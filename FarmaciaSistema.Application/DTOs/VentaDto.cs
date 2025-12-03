namespace FarmaciaSistema.Application.DTOs
{
    public class VentaDto
    {
        public decimal Total { get; set; }
        public int UsuarioId { get; set; }
        public List<DetalleVentaDto> Detalles { get; set; }
    }

    public class DetalleVentaDto
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }
}
