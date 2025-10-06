using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Windows;
using System.Text;

namespace FarmaciaSistema.Desktop
{
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _httpClient;

        public class UserViewModel
        {
            public int Id { get; set; }
            public string NombreUsuario { get; set; }
        }

        public LoginWindow()
        {
            InitializeComponent(); // <-- Este es el método que da el error

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7076/"); // ¡Recuerda cambiar tu puerto!
            this.Loaded += LoginWindow_Loaded;
        }

        private async void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/Usuarios/Nombres");
                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var usuarios = JsonConvert.DeserializeObject<List<UserViewModel>>(jsonString);
                    UsuariosComboBox.ItemsSource = usuarios;
                }
                else
                {
                    MessageBox.Show("No se pudieron cargar los usuarios. Código de error: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo conectar con el servidor: {ex.Message}");
            }
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedUser = UsuariosComboBox.SelectedItem as UserViewModel;
            if (selectedUser == null)
            {
                MessageBox.Show("Por favor, seleccione un usuario.");
                return;
            }

            var password = PasswordInput.Password;
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Por favor, ingrese una contraseña.");
                return;
            }

            // 1. Prepara los datos para enviar a la API.
            var loginData = new
            {
                NombreUsuario = selectedUser.NombreUsuario,
                Password = password
            };

            // 2. Convierte los datos a formato JSON.
            var jsonContent = JsonConvert.SerializeObject(loginData);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // 3. Llama al endpoint de la API.
            try
            {
                var response = await _httpClient.PostAsync("api/Usuarios/Login", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    // Si la API responde "Ok", el login es correcto.
                    MessageBox.Show("¡Inicio de sesión exitoso!");

                    // Aquí es donde abrimos la ventana principal.
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close(); // Cierra la ventana de login.
                }
                else
                {
                    // Si la API responde "Unauthorized", las credenciales son incorrectas.
                    MessageBox.Show("Error: Credenciales inválidas.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo conectar con el servidor: {ex.Message}");
            }
        }
    }
}