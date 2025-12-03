using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FarmaciaSistema.Desktop
{
    public partial class CitasWindow : Window
    {
        private readonly HttpClient _httpClient;

        // Reutilizamos la clase Cliente que ya tienes en el Desktop
        public CitasWindow()
        {
            InitializeComponent();
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("https://localhost:7076/"); // ¡Verifica el puerto!
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarPacientes();
        }

        private async Task CargarPacientes()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Clientes");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var pacientes = JsonConvert.DeserializeObject<List<Cliente>>(json);
                    CboPacientes.ItemsSource = pacientes;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar pacientes: " + ex.Message);
            }
        }

        private async void BtnGuardarCita_Click(object sender, RoutedEventArgs e)
        {
            if (CboPacientes.SelectedValue == null)
            {
                MessageBox.Show("Por favor seleccione un paciente.");
                return;
            }
            if (string.IsNullOrWhiteSpace(TxtSintomas.Text) || string.IsNullOrWhiteSpace(TxtReceta.Text))
            {
                MessageBox.Show("Debe llenar los síntomas y la receta.");
                return;
            }

            var nuevaCita = new
            {
                ClienteId = (int)CboPacientes.SelectedValue,
                Sintomas = TxtSintomas.Text,
                Receta = TxtReceta.Text,
                Fecha = DateTime.Now
            };

            try
            {
                var json = JsonConvert.SerializeObject(nuevaCita);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("api/Citas", content);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Consulta guardada correctamente.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error al guardar la consulta.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
            }
        }
    }
}
