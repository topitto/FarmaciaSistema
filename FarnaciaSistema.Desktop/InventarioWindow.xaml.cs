using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FarmaciaSistema.Desktop
{
    public partial class InventarioWindow : Window
    {
        private readonly HttpClient _httpClient;

        public InventarioWindow()
        {
            InitializeComponent();

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("https://localhost:7076/"); // ¡Recuerda cambiar tu puerto!
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarProductos();
        }

        private async Task CargarProductos()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Productos");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var productos = JsonConvert.DeserializeObject<List<Producto>>(jsonString);
                    ProductosGrid.ItemsSource = productos;
                }
                else
                {
                    MessageBox.Show("Error al cargar los productos.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexión: {ex.Message}");
            }
        }

        private void ProductosGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProductosGrid.SelectedItem is Producto producto)
            {
                TxtId.Text = producto.Id.ToString();
                TxtNombre.Text = producto.Nombre;
                TxtDescripcion.Text = producto.Descripcion;
                TxtPrecio.Text = producto.Precio.ToString();
                TxtStock.Text = producto.Stock.ToString();
                DpFechaCaducidad.SelectedDate = producto.FechaCaducidad;
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            TxtId.Text = string.Empty;
            TxtNombre.Text = string.Empty;
            TxtDescripcion.Text = string.Empty;
            TxtPrecio.Text = string.Empty;
            TxtStock.Text = string.Empty;
            DpFechaCaducidad.SelectedDate = null;
            ProductosGrid.SelectedItem = null;
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Validaciones básicas de entrada
            if (!decimal.TryParse(TxtPrecio.Text, out decimal precio))
            {
                MessageBox.Show("El precio no es válido.");
                return;
            }
            if (!int.TryParse(TxtStock.Text, out int stock))
            {
                MessageBox.Show("El stock no es válido.");
                return;
            }
            if (DpFechaCaducidad.SelectedDate == null)
            {
                MessageBox.Show("Por favor, seleccione una fecha de caducidad.");
                return;
            }

            var producto = new Producto
            {
                Nombre = TxtNombre.Text,
                Descripcion = TxtDescripcion.Text,
                Precio = precio,
                Stock = stock,
                FechaCaducidad = DpFechaCaducidad.SelectedDate.Value
            };

            try
            {
                HttpResponseMessage response;
                if (string.IsNullOrEmpty(TxtId.Text))
                {
                    // --- CREAR (POST) ---
                    var jsonContent = JsonConvert.SerializeObject(producto);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync("api/Productos", httpContent);
                }
                else
                {
                    // --- ACTUALIZAR (PUT) ---
                    producto.Id = int.Parse(TxtId.Text);
                    var jsonContent = JsonConvert.SerializeObject(producto);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    response = await _httpClient.PutAsync($"api/Productos/{producto.Id}", httpContent);
                }

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Error al guardar el producto.");
                }

                await CargarProductos();
                BtnLimpiar_Click(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexión: {ex.Message}");
            }
        }

        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtId.Text))
            {
                MessageBox.Show("Por favor, seleccione un producto para eliminar.");
                return;
            }

            var id = TxtId.Text;
            var confirm = MessageBox.Show($"¿Seguro que desea eliminar el producto ID {id}?", "Confirmar Eliminación", MessageBoxButton.YesNo);

            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"api/Productos/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Error al eliminar el producto.");
                    }
                    await CargarProductos();
                    BtnLimpiar_Click(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error de conexión: {ex.Message}");
                }
            }
        }
    }
}
