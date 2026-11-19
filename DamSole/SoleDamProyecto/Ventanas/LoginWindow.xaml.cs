
using System.Windows;
using System.Windows.Controls;

namespace SoleDamProyecto.Ventanas
{
    /// <summary>
    /// Lógica de interacción para LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private bool esPasswordVisible = false;

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void TxtPass_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TxtPass.Password))
            {
                TxtPassHint.Visibility = Visibility.Visible;
            }
            else
            {
                TxtPassHint.Visibility = Visibility.Collapsed;
            }

            if (!esPasswordVisible)
            {
                TxtPassVisible.Text = TxtPass.Password;
            }
        }

        private void TxtPassVisible_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (esPasswordVisible)
            {
                TxtPass.Password = TxtPassVisible.Text;
            }
        }

        private void BtnOjo_Click(object sender, RoutedEventArgs e)
        {
            esPasswordVisible = BtnOjo.IsChecked ?? false;

            if (esPasswordVisible)
            {
                TxtPassVisible.Text = TxtPass.Password;
                TxtPass.Visibility = Visibility.Collapsed;
                TxtPassVisible.Visibility = Visibility.Visible;
                TxtPassVisible.Focus();
                TxtPassVisible.CaretIndex = TxtPassVisible.Text.Length;
            }
            else
            {
                TxtPass.Password = TxtPassVisible.Text;
                TxtPassVisible.Visibility = Visibility.Collapsed;
                TxtPass.Visibility = Visibility.Visible;
                TxtPass.Focus();
            }
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            // Puedes añadir aquí tu validación rápida de usuario/contraseña si lo deseas.
            // Al pulsar el botón, abrimos la ventana HomeWindow y cerramos el Login:

            HomeWindow ventanaHome = new HomeWindow();
            ventanaHome.Show();
            this.Close();
        }
    }
}