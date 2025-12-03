using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel; // Necesario para listas dinámicas
using System.Linq; // Necesario para buscar en listas
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input; // Necesario para detectar teclas

namespace FarmaciaSistema.Desktop
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient;

        // Esta lista especial avisa a la pantalla cuando cambian los datos
        private ObservableCollection<VentaItem> _itemsVenta;

        public MainWindow()
        {
            InitializeComponent();

            // Configuración del cliente HTTP (igual que en las otras ventanas)
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = new Uri("https://localhost:7076/"); // ¡VERIFICA TU PUERTO!

            // Inicializamos la lista de venta vacía
            _itemsVenta = new ObservableCollection<VentaItem>();
            ListaVenta.ItemsSource = _itemsVenta; // Conectamos la lista visual con los datos

            // Evento para detectar "Enter" en la caja de texto
            TxtCodigoProducto.KeyDown += TxtCodigoProducto_KeyDown;

            // Poner el foco en la caja de texto al iniciar
            TxtCodigoProducto.Focus();
        }

        private async void TxtCodigoProducto_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string codigo = TxtCodigoProducto.Text.Trim();
                if (!string.IsNullOrEmpty(codigo))
                {
                    await AgregarProductoAVenta(codigo);
                    TxtCodigoProducto.Text = ""; // Limpiar caja
                    TxtCodigoProducto.Focus();   // Regresar foco
                }
            }
        }

        private async Task AgregarProductoAVenta(string codigo)
        {
            // Intentamos convertir el código a ID (porque nuestra API busca por ID numérico por ahora)
            if (!int.TryParse(codigo, out int productoId))
            {
                MessageBox.Show("Por favor ingrese un ID numérico válido.");
                return;
            }

            try
            {
                // 1. Buscamos el producto en la API
                var response = await _httpClient.GetAsync($"api/Productos/{productoId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var producto = JsonConvert.DeserializeObject<Producto>(jsonString);

                    // 2. Verificamos si ya está en la lista para solo sumar cantidad
                    var itemExistente = _itemsVenta.FirstOrDefault(i => i.ProductoId == producto.Id);

                    if (itemExistente != null)
                    {
                        // Si ya existe, aumentamos la cantidad
                        itemExistente.Cantidad++;
                        // Truco para refrescar la lista visualmente en WPF:
                        ListaVenta.Items.Refresh();
                    }
                    else
                    {
                        // Si no existe, creamos una nueva línea
                        var nuevoItem = new VentaItem
                        {
                            ProductoId = producto.Id,
                            Nombre = producto.Nombre,
                            PrecioUnitario = producto.Precio,
                            Cantidad = 1
                        };
                        _itemsVenta.Add(nuevoItem);
                    }
                }
                else
                {
                    MessageBox.Show("Producto no encontrado o sin stock.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error de conexión: {ex.Message}");
            }
        }

        // --- MÉTODOS DE NAVEGACIÓN DEL MENÚ ---
        private void BtnInventario_Click(object sender, RoutedEventArgs e)
        {
            new InventarioWindow().Show();
        }

        private void BtnProveedores_Click(object sender, RoutedEventArgs e)
        {
            new ProveedoresWindow().Show();
        }

        private void BtnClientes_Click(object sender, RoutedEventArgs e)
        {
            new ClientesWindow().Show();
        }
        private void BtnEliminarArticulo_Click(object sender, RoutedEventArgs e)
        {
            // Verificamos si hay algo seleccionado en la lista visual
            if (ListaVenta.SelectedItem is VentaItem itemSeleccionado)
            {
                // Lo borramos de la lista de datos (_itemsVenta)
                // La lista visual se actualiza sola gracias a ObservableCollection
                _itemsVenta.Remove(itemSeleccionado);

                // Ponemos el foco de nuevo en la caja para seguir escaneando rápido
                TxtCodigoProducto.Focus();
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto de la lista para eliminarlo.");
            }
        }
        private void BtnRepetirArticulo_Click(object sender, RoutedEventArgs e)
        {
            if (ListaVenta.SelectedItem is VentaItem itemSeleccionado)
            {
                // Simplemente aumentamos la cantidad
                itemSeleccionado.Cantidad++;

                // Refrescamos la vista para que se actualice el Subtotal visualmente
                ListaVenta.Items.Refresh();

                TxtCodigoProducto.Focus();
            }
            else
            {
                MessageBox.Show("Por favor, selecciona un producto de la lista para repetir.");
            }
        }
        private void BtnSubTotal_Click(object sender, RoutedEventArgs e)
        {
            if (_itemsVenta.Count == 0)
            {
                MessageBox.Show("No hay productos en la venta.");
                return;
            }

            // ABRIR VENTANA DE COBRO
            // Le pasamos la lista de items (_itemsVenta) al constructor
            CobroWindow cobroWindow = new CobroWindow(_itemsVenta);
            cobroWindow.ShowDialog(); // ShowDialog bloquea la ventana principal hasta que cierres esta
        }
        private void BtnCitas_Click(object sender, RoutedEventArgs e)
        {
            new CitasWindow().Show();
        }

        private void BtnUsuarios_Click(object sender, RoutedEventArgs e)
        {
            new UsuariosWindow().Show();
        }

        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("¿Está seguro que desea cerrar sesión?", "Cerrar Sesión",
        MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                // 1. Crear una nueva instancia de la ventana de Login
                LoginWindow login = new LoginWindow();

                // 2. Mostrar el Login
                login.Show();

                // 3. Cerrar la ventana principal actual (MainWindow)
                this.Close();
            }
        }
    }
}
