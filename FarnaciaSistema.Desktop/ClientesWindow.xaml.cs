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
    public partial class ClientesWindow : Window
    {
        private readonly HttpClient _httpClient;

        public ClientesWindow()
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
            await CargarClientes();
        }

        private async Task CargarClientes()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Clientes");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var clientes = JsonConvert.DeserializeObject<List<Cliente>>(jsonString);
                    ClientesGrid.ItemsSource = clientes;
                }
                else
                {
                    MessageBox.Show("Error al cargar los clientes.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexión: {ex.Message}");
            }
        }

        private void ClientesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ClientesGrid.SelectedItem is Cliente cliente)
            {
                TxtId.Text = cliente.Id.ToString();
                TxtNombre.Text = cliente.Nombre;
                TxtTelefono.Text = cliente.Telefono;
                TxtRfc.Text = cliente.RFC;
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            TxtId.Text = string.Empty;
            TxtNombre.Text = string.Empty;
            TxtTelefono.Text = string.Empty;
            TxtRfc.Text = string.Empty;
            ClientesGrid.SelectedItem = null;
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            var cliente = new Cliente
            {
                Nombre = TxtNombre.Text,
                Telefono = TxtTelefono.Text,
                RFC = TxtRfc.Text
            };

            try
            {
                HttpResponseMessage response;
                if (string.IsNullOrEmpty(TxtId.Text))
                {
                    // --- CREAR (POST) ---
                    var jsonContent = JsonConvert.SerializeObject(cliente);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync("api/Clientes", httpContent);
                }
                else
                {
                    // --- ACTUALIZAR (PUT) ---
                    cliente.Id = int.Parse(TxtId.Text);
                    var jsonContent = JsonConvert.SerializeObject(cliente);
                    var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                    response = await _httpClient.PutAsync($"api/Clientes/{cliente.Id}", httpContent);
                }

                if (!response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Error al guardar el cliente.");
                }

                await CargarClientes();
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
                MessageBox.Show("Por favor, seleccione un cliente para eliminar.");
                return;
            }

            var id = TxtId.Text;
            var confirm = MessageBox.Show($"¿Seguro que desea eliminar el cliente ID {id}?", "Confirmar Eliminación", MessageBoxButton.YesNo);

            if (confirm == MessageBoxResult.Yes)
            {
                try
                {
                    var response = await _httpClient.DeleteAsync($"api/Clientes/{id}");
                    if (!response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Error al eliminar el cliente.");
                    }
                    await CargarClientes();
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
