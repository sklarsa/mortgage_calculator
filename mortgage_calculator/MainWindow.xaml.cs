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

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog d = new Microsoft.Win32.SaveFileDialog();

            d.AddExtension = true;
            d.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            d.OverwritePrompt = true;
            d.CheckPathExists = true;
            d.DefaultExt = ".xlsx";
            d.Filter = "Excel 2007-2013 Files|*.xlsx";
            d.FileOk += (s1, e1) =>
            {
                Mortgage mtge = this.DataContext as Mortgage;
                if (mtge != null)
                {
                    mtge.ExportToExcel(d.FileName);
                }
            };

            d.ShowDialog();

        }

        private void btnExportCsv_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog d = new Microsoft.Win32.SaveFileDialog();

            d.AddExtension = true;
            d.InitialDirectory = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            d.OverwritePrompt = true;
            d.CheckPathExists = true;
            d.DefaultExt = ".csv";
            d.Filter = "Comma-Delimited FIles|*.csv";
            d.FileOk += (s1, e1) =>
            {
                Mortgage mtge = this.DataContext as Mortgage;
                if (mtge != null)
                {
                    mtge.ExportToCSV(d.FileName);
                }
            };

            d.ShowDialog();


        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            e.Handled = true;
        }
    }
}
