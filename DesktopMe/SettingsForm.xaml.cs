using HandyControl.Controls;

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
using System.Windows.Shapes;

namespace DesktopMe {
    /// <summary>
    /// Логика взаимодействия для SettingsForm.xaml
    /// </summary>
    public partial class SettingsForm : System.Windows.Window {
        private readonly MainWindow mainWindow;
        public SettingsForm(MainWindow mainWindow) {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e) {
            IniFile iniFile = new IniFile("file_name.ini");
            iniFile.Write("Width", width.Value.ToString(), "Size");
            iniFile.Write("Height", height.Value.ToString(), "Size");


            this.DialogResult = true;
            this.Close();

        }

        private void Cancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void NumericUpDown_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e) {
            mainWindow.Height = (sender as NumericUpDown).Value;
        }

        private void NumericUpDown_ValueChanged_1(object sender, HandyControl.Data.FunctionEventArgs<double> e) {
            mainWindow.Width = (sender as NumericUpDown).Value;

        }
    }
}
