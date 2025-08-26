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

namespace NorModifierCod3rWPF.MessageBox
{
    /// <summary>
    /// Interaction logic for MessageBox.xaml
    /// </summary>
    public partial class MessageBox : Page
    {
        public MessageBox(string error, string errordescription)
        {
            InitializeComponent();
            DisplayUI(error, errordescription);
        }

        private void DisplayUI(string error, string errordescription)
        {
            try
            {
                HeaderText.Text = error;
                MessageDescription.Text = errordescription;
            }
            catch
            {
                HeaderText.Text = "Error";
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.CloseMessage();
            }
        }
    }
}
