using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NorModifierCod3rWPF.Pages.SubPages
{
    /// <summary>
    /// Interaction logic for UARTCom.xaml
    /// </summary>
    public partial class UARTCom : Page
    {
        public UARTCom()
        {
            InitializeComponent();
        }

        private void DonateToCod3r(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.OpenDonos();
            }
        }
    }
}
