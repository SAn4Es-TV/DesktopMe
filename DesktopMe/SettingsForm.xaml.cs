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
            IniFile iniFile = new IniFile("file_name.ini");
            InitializeComponent();
            width.Value = iniFile.ReadInt("Width", "Size");
            height.Value = iniFile.ReadInt("Height", "Size");
            sensivity.Value = iniFile.ReadInt("Sensivity", "Other");
            sensyvText.Text = "Чувствительность: " + iniFile.ReadInt("Sensivity", "Other");
        }

        private void Save_Click(object sender, RoutedEventArgs e) {
            IniFile iniFile = new IniFile("file_name.ini");
            iniFile.Write("Width", width.Value.ToString(), "Size");
            iniFile.Write("Height", height.Value.ToString(), "Size");
            iniFile.Write("Sensivity", ((int)sensivity.Value).ToString(), "Other");

            iniFile.Write("BlinkMin", ((int)blinkTimeMin.Value).ToString(), "Other");
            iniFile.Write("BlinkMax", ((int)blinkTimeMax.Value).ToString(), "Other");
            iniFile.Write("BlinkTime", ((int)blinkTime.Value).ToString(), "Other");


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

        private void sensivity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
            mainWindow.sensivity = (int)(sender as Slider).Value;
            sensyvText.Text = "Чувствительность: " + (int)sensivity.Value;
        }

        private void blinkTime_ValueChanged(object sender, HandyControl.Data.FunctionEventArgs<double> e) {
            mainWindow.eyeClosedTime = (int)(sender as NumericUpDown).Value;
        }
    }
}
