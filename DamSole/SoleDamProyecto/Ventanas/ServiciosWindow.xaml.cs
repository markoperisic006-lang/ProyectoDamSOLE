using System;
using System.Windows;
using System.Windows.Controls;

namespace SoleDamProyecto.Ventanas
{
    /// <summary>Formulario de servicios: permite registrar servicios y ver los ya guardados.</summary>
    public partial class ServiciosWindow : Window
    {
        public ServiciosWindow()
        {
            InitializeComponent();
        }

        /// <summary>Devuelve el texto de la prioridad elegida en los botones de opción.</summary>
        private string PrioridadElegida()
        {
            if (PrioridadBajaRadio.IsChecked == true) return "Baja";
            if (PrioridadAltaRadio.IsChecked == true) return "Alta";
            return "Media";
        }

        /// <summary>Evento: valida el formulario y añade el servicio a la lista.</summary>
        private void GuardarServicioBoto_Click(object sender, RoutedEventArgs e)
        {
            string nombre = NombreServicioText.Text.Trim();

            if (nombre.Length == 0)
            {
                EstadoServicioText.Text = "Escribe el nombre del servicio.";
                NombreServicioText.Focus();
                return;
            }

            if (TipoServicioCombo.SelectedItem == null)
            {
                EstadoServicioText.Text = "Selecciona el tipo de servicio.";
                TipoServicioCombo.Focus();
                return;
            }

            if (FechaServicioFecha.SelectedDate == null)
            {
                EstadoServicioText.Text = "Selecciona la fecha de inicio.";
                FechaServicioFecha.Focus();
                return;
            }

            string tipo = ((ComboBoxItem)TipoServicioCombo.SelectedItem).Content.ToString();
            string fecha = FechaServicioFecha.SelectedDate.Value.ToString("dd/MM/yyyy");
            string urgente = UrgenteCheck.IsChecked == true ? " · URGENTE" : "";

            ServiciosLista.Items.Add(nombre + " (" + tipo + ") - " + fecha + " - Prioridad " + PrioridadElegida() + urgente);

            LimpiarFormulario();
            EstadoServicioText.Foreground = System.Windows.Media.Brushes.SeaGreen;
            EstadoServicioText.Text = "Servicio guardado correctamente.";
        }

        /// <summary>Deja el formulario vacío y con los valores por defecto.</summary>
        private void LimpiarFormulario()
        {
            NombreServicioText.Clear();
            DescripcionServicioText.Clear();
            TipoServicioCombo.SelectedIndex = -1;
            FechaServicioFecha.SelectedDate = null;
            PrioridadMediaRadio.IsChecked = true;
            UrgenteCheck.IsChecked = false;
        }

        private void LimpiarServicioBoto_Click(object sender, RoutedEventArgs e)
        {
            LimpiarFormulario();
            EstadoServicioText.Text = "";
        }

        /// <summary>Evento: solo se puede eliminar cuando hay un servicio seleccionado.</summary>
        private void ServiciosLista_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EliminarServicioBoto.IsEnabled = ServiciosLista.SelectedItem != null;
        }

        private void EliminarServicioBoto_Click(object sender, RoutedEventArgs e)
        {
            if (ServiciosLista.SelectedItem != null)
            {
                ServiciosLista.Items.Remove(ServiciosLista.SelectedItem);
                EstadoServicioText.Foreground = System.Windows.Media.Brushes.SeaGreen;
                EstadoServicioText.Text = "Servicio eliminado.";
            }
        }

        /// <summary>Evento: vuelve a la pantalla Home y cierra esta ventana.</summary>
        private void TornarInicioBoto_Click(object sender, RoutedEventArgs e)
        {
            HomeWindow ventanaHome = new HomeWindow();
            ventanaHome.Show();
            Close();
        }
    }
}