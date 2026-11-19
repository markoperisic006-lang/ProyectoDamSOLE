using System.Windows;

namespace SoleDamProyecto.Ventanas
{
    /// <summary>
    /// Lógica de interacción para HomeWindow.xaml
    /// </summary>
    public partial class HomeWindow : Window
    {
        public HomeWindow()
        {
            InitializeComponent();
        }

        private void SeleccionarDiesBoto_Click(object sender, RoutedEventArgs e)
        {
            CalendarioWindow ventanaCalendario = new CalendarioWindow();
            ventanaCalendario.Show();
            this.Close();
        }

        private void ServicisBoto_Click(object sender, RoutedEventArgs e)
        {
            ServiciosWindow ventanaServicios = new ServiciosWindow();
            ventanaServicios.Show();
            this.Close();
        }

        private void AprobarSolicitudesBoto_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Abriendo panel de Aprobación de Solicitudes...", "Gestión de Dirección", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CargarDiasFB_Boto_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Abriendo panel de Carga de Días Festivos y Bloqueados...", "Configuración de Calendario", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EixirAplicacioBoto_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}