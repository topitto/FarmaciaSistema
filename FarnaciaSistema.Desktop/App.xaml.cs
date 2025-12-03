using System.Configuration;
using System.Data;
using System.Windows;

namespace FarmaciaSistema.Desktop
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Licencia comunitaria (gratis)
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }
    }

}
