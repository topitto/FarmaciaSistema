using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions; // Para validar números
using System.Windows;
using System.Windows.Input;

namespace FarmaciaSistema.Desktop
{
    public partial class CobroWindow : Window
    {
        private readonly HttpClient _httpClient;
        private ObservableCollection<VentaItem> _items;
        private decimal _totalVenta;

        // Constructor modificado: Recibe la lista de productos
        public CobroWindow(ObservableCollection<VentaItem> items)
        {
            InitializeComponent();
            _items = items;

            // Configurar HttpClient
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("https://localhost:7076/"); // ¡Tu puerto!

            CargarDatos();
        }

        private void CargarDatos()
        {
            // Llenar la lista visual
            ListaResumen.ItemsSource = _items;

            // Calcular el total
            _totalVenta = _items.Sum(i => i.Subtotal);
            TxtTotalPagar.Text = _totalVenta.ToString("C2");

            // Enfocar la caja de efectivo
            TxtEfectivo.Focus();
        }

        // Validación para que solo escriban números y puntos decimales
        private void TxtEfectivo_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Calcular el cambio en tiempo real
        private void TxtEfectivo_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (decimal.TryParse(TxtEfectivo.Text, out decimal efectivo))
            {
                decimal cambio = efectivo - _totalVenta;
                TxtCambio.Text = cambio.ToString("C2");

                // Solo habilitar el botón si el efectivo alcanza
                if (cambio >= 0)
                {
                    BtnFinalizar.IsEnabled = true;
                    TxtCambio.Foreground = System.Windows.Media.Brushes.Green;
                }
                else
                {
                    BtnFinalizar.IsEnabled = false;
                    TxtCambio.Foreground = System.Windows.Media.Brushes.Red;
                }
            }
            else
            {
                TxtCambio.Text = "$0.00";
                BtnFinalizar.IsEnabled = false;
            }
        }

        private async void BtnFinalizar_Click(object sender, RoutedEventArgs e)
        {
            // Preparamos el objeto para enviar a la API
            // (Necesitaremos crear este DTO en el siguiente paso)
            var ventaDto = new
            {
                Total = _totalVenta,
                UsuarioId = 1, // Por ahora fijo, idealmente vendría del Login
                Detalles = _items.Select(i => new
                {
                    ProductoId = i.ProductoId,
                    Cantidad = i.Cantidad,
                    PrecioUnitario = i.PrecioUnitario
                }).ToList()
            };

            try
            {
                var json = JsonConvert.SerializeObject(ventaDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/Ventas", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show($"¡Venta realizada con éxito!\nCambio a entregar: {TxtCambio.Text}", "Venta Finalizada");

                    // Limpiamos la lista original (que viene de MainWindow)
                    _items.Clear();

                    this.Close(); // Cerramos la ventana de cobro
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al procesar la venta: {error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
            }
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Solo cierra esta ventana, no borra la venta
        }
    }
}
