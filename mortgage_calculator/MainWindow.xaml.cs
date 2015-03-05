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
using mortgage_calculator.Model;

namespace mortgage_calculator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new Mortgage();
        }

        private void btnCalculate_Click(object sender, RoutedEventArgs e)
        {
            Mortgage mtge = this.DataContext as Mortgage;
            if (mtge != null)
            {
                mtge.Calculate();
            }
        }
    }
}
