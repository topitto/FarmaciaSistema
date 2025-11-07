using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace FarmaciaSistema.Desktop
{
    /// <summary>
    /// Lógica de interacción para ProveedoresWindow.xaml
    /// </summary>
    public partial class ProveedoresWindow : Window
    {
        private readonly HttpClient _httpClient;
        public ProveedoresWindow()
        {
            InitializeComponent();

            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("https://localhost:7076/");
        }
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Llama al método para cargar los datos cuando la ventana se abre
            await CargarProveedores();
        }
        private async Task CargarProveedores()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Proveedores");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var proveedores = JsonConvert.DeserializeObject<List<Proveedor>>(jsonString);
                    ProveedoresGrid.ItemsSource = proveedores;
                }
                else
                {
                    MessageBox.Show("Error al cargar los proveedores.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexión: {ex.Message}");
            }
        }
        private void ProveedoresGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Esta lógica llenará el formulario cuando selecciones un ítem
            if (ProveedoresGrid.SelectedItem is Proveedor proveedor)
            {
                TxtId.Text = proveedor.Id.ToString();
                TxtNombre.Text = proveedor.Nombre;
                TxtContacto.Text = proveedor.Contacto;
                TxtTelefono.Text = proveedor.Telefono;
            }
        }
        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            // Prepara el objeto Proveedor con los datos del formulario
            var proveedor = new Proveedor
            {
                Nombre = TxtNombre.Text,
                Contacto = TxtContacto.Text,
                Telefono = TxtTelefono.Text
            };

            try
            {
                if (string.IsNullOrEmpty(TxtId.Text))
                {
                    // --- CREAR (POST) ---
                    var jsonContent = JsonConvert.SerializeObject(proveedor);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PostAsync("api/Proveedores", httpContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Error al crear el proveedor.");
                    }
                }
                else
                {
                    // --- ACTUALIZAR (PUT) ---
                    proveedor.Id = int.Parse(TxtId.Text); // Asigna el ID para la actualización
                    var jsonContent = JsonConvert.SerializeObject(proveedor);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var response = await _httpClient.PutAsync($"api/Proveedores/{proveedor.Id}", httpContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Error al actualizar el proveedor.");
                    }
                }

                // Refresca la lista y limpia el formulario
                await CargarProveedores();
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
                MessageBox.Show("Por favor, seleccione un proveedor de la lista para eliminar.");
                return;
            }

            var id = TxtId.Text;

            // Pedimos confirmación
            var confirm = MessageBox.Show($"¿Está seguro de que desea eliminar al proveedor ID {id}?", "Confirmar Eliminación", MessageBoxButton.YesNo);

            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"api/Proveedores/{id}");

                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Error al eliminar el proveedor.");
                    }

                    // Refresca la lista y limpia el formulario
                    await CargarProveedores();
                    BtnLimpiar_Click(null, null);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error de conexión: {ex.Message}");
                }
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            // Lógica para limpiar el formulario
            TxtId.Text = string.Empty;
            TxtNombre.Text = string.Empty;
            TxtContacto.Text = string.Empty;
            TxtTelefono.Text = string.Empty;
            ProveedoresGrid.SelectedItem = null;
        }
    }
}
