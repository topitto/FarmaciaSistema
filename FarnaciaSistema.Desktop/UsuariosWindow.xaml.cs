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
    public partial class UsuariosWindow : Window
    {
        private readonly HttpClient _httpClient;

        public UsuariosWindow()
        {
            InitializeComponent();
            var handler = new HttpClientHandler { ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator };
            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("https://localhost:7076/"); // ¡Tu puerto!
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await CargarUsuarios();
        }

        private async Task CargarUsuarios()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Usuarios");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var usuarios = JsonConvert.DeserializeObject<List<Usuario>>(json);
                    UsuariosGrid.ItemsSource = usuarios;
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private void UsuariosGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (UsuariosGrid.SelectedItem is Usuario u)
            {
                TxtId.Text = u.Id.ToString();
                TxtNombreUsuario.Text = u.NombreUsuario;
                CboRol.Text = u.Rol;
                TxtPassword.Password = ""; // Limpiamos la caja de contraseña por seguridad
            }
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            var usuario = new Usuario
            {
                NombreUsuario = TxtNombreUsuario.Text,
                Rol = CboRol.Text,
                PasswordHash = TxtPassword.Password
            };

            try
            {
                HttpResponseMessage response;
                if (string.IsNullOrEmpty(TxtId.Text))
                {
                    // CREAR
                    if (string.IsNullOrEmpty(usuario.PasswordHash))
                    {
                        MessageBox.Show("La contraseña es obligatoria para nuevos usuarios."); return;
                    }
                    var json = JsonConvert.SerializeObject(usuario);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync("api/Usuarios", content);
                }
                else
                {
                    // EDITAR
                    usuario.Id = int.Parse(TxtId.Text);
                    var json = JsonConvert.SerializeObject(usuario);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await _httpClient.PutAsync($"api/Usuarios/{usuario.Id}", content);
                }

                if (response.IsSuccessStatusCode)
                {
                    await CargarUsuarios();
                    BtnLimpiar_Click(null, null);
                }
                else MessageBox.Show("Error al guardar usuario.");
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }

        private async void BtnEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtId.Text)) return;
            var id = TxtId.Text;
            if (MessageBox.Show("¿Eliminar usuario?", "Confirmar", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                var response = await _httpClient.DeleteAsync($"api/Usuarios/{id}");
                if (response.IsSuccessStatusCode) { await CargarUsuarios(); BtnLimpiar_Click(null, null); }
            }
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            TxtId.Text = ""; TxtNombreUsuario.Text = ""; TxtPassword.Password = ""; CboRol.SelectedIndex = -1; UsuariosGrid.SelectedItem = null;
        }
    }
}
