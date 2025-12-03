using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FarmaciaSistema.Desktop.Services;

namespace FarmaciaSistema.Desktop
{
    public partial class CitasWindow : Window
    {
        private readonly HttpClient _httpClient;

        public CitasWindow()
        {
            InitializeComponent();
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("https://localhost:7076/"); // ¡Confirma tu puerto!
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarPacientes();
        }

        // --- Lógica Visual: Cambiar entre Nuevo y Existente ---
        private void RbTipoPaciente_Checked(object sender, RoutedEventArgs e)
        {
            // Verificamos que los controles ya se hayan cargado para evitar errores al inicio
            if (PanelPacienteExistente == null || PanelPacienteNuevo == null) return;

            if (RbExistente.IsChecked == true)
            {
                PanelPacienteExistente.Visibility = Visibility.Visible;
                PanelPacienteNuevo.Visibility = Visibility.Collapsed;
            }
            else
            {
                PanelPacienteExistente.Visibility = Visibility.Collapsed;
                PanelPacienteNuevo.Visibility = Visibility.Visible;
            }
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
            // Validar campos comunes (Síntomas y Receta)
            if (string.IsNullOrWhiteSpace(TxtSintomas.Text) || string.IsNullOrWhiteSpace(TxtReceta.Text))
            {
                MessageBox.Show("Debe llenar los síntomas y la receta.");
                return;
            }

            int clienteIdParaLaCita = 0;

            try
            {
                // --- PASO 1: Obtener el ID del Cliente ---
                if (RbNuevo.IsChecked == true)
                {
                    // LÓGICA PARA NUEVO PACIENTE
                    if (string.IsNullOrWhiteSpace(TxtNuevoNombre.Text))
                    {
                        MessageBox.Show("Debe escribir el nombre del nuevo paciente.");
                        return;
                    }

                    // 1. Crear el objeto del nuevo cliente
                    var nuevoCliente = new Cliente
                    {
                        Nombre = TxtNuevoNombre.Text,
                        Telefono = TxtNuevoTelefono.Text,
                        RFC = TxtNuevoRfc.Text,
                        Email = TxtNuevoEmail.Text
                    };

                    // 2. Guardarlo en la API de Clientes
                    var jsonCliente = JsonConvert.SerializeObject(nuevoCliente);
                    var contentCliente = new StringContent(jsonCliente, Encoding.UTF8, "application/json");
                    var responseCliente = await _httpClient.PostAsync("api/Clientes", contentCliente);

                    if (!responseCliente.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Error al registrar el nuevo paciente.");
                        return;
                    }

                    // 3. Leer la respuesta para obtener el ID recién creado
                    var jsonRespuesta = await responseCliente.Content.ReadAsStringAsync();
                    var clienteCreado = JsonConvert.DeserializeObject<Cliente>(jsonRespuesta);
                    clienteIdParaLaCita = clienteCreado.Id;
                }
                else
                {
                    // LÓGICA PARA PACIENTE EXISTENTE
                    if (CboPacientes.SelectedValue == null)
                    {
                        MessageBox.Show("Por favor seleccione un paciente de la lista.");
                        return;
                    }
                    clienteIdParaLaCita = (int)CboPacientes.SelectedValue;
                }

                // --- PASO 2: Guardar la Cita con el ID obtenido ---
                var nuevaCita = new
                {
                    ClienteId = clienteIdParaLaCita,
                    Sintomas = TxtSintomas.Text,
                    Receta = TxtReceta.Text,
                    Fecha = DateTime.Now
                };

                var jsonCita = JsonConvert.SerializeObject(nuevaCita);
                var contentCita = new StringContent(jsonCita, Encoding.UTF8, "application/json");

                var responseCita = await _httpClient.PostAsync("api/Citas", contentCita);

                if (responseCita.IsSuccessStatusCode)
                {
                    // --- GENERAR PDF ---
                    var pdfService = new RecetaPdfService();

                    // Obtenemos el nombre del paciente (ya sea del combo o del textbox nuevo)
                    string nombrePaciente = RbNuevo.IsChecked == true ? TxtNuevoNombre.Text : ((Cliente)CboPacientes.SelectedItem).Nombre;

                    pdfService.GenerarReceta(nombrePaciente, TxtSintomas.Text, TxtReceta.Text, DateTime.Now.ToString("g"));

                    MessageBox.Show("Consulta guardada y PDF generado exitosamente.");
                    this.Close();
                }
                else
                {
                    // Leemos qué nos respondió el servidor
                    var errorDetalle = await responseCita.Content.ReadAsStringAsync();
                    MessageBox.Show($"Error al guardar ({responseCita.StatusCode}): {errorDetalle}");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error de conexión: " + ex.Message);
            }
        }
    }
}
