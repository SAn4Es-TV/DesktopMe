using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    /// Логика взаимодействия для AccessorieWindow.xaml
    /// </summary>
    public partial class AccessorieWindow : Window {
        bool pinned = false;
        int size = 300;

        private readonly MainWindow _previous;
        Accessorie accessorie;

        List<BitmapImage> bitmapImages = new List<BitmapImage>();
        public AccessorieWindow(Accessorie accessorie_, MainWindow window, bool pinned_ = false) {
            InitializeComponent();
            pinned = pinned_;
            accessorie = accessorie_;
            _previous = window;
            Height = accessorie.Size;
            Width = accessorie.Size;
            //main.Source = new BitmapImage(new Uri(accessorie.Path));

            DirectoryInfo directoryInfo = new DirectoryInfo(accessorie_.Path);
            for (int i = 1; i <= directoryInfo.GetFiles().Length; i++) {
                this.Dispatcher.Invoke(() => {
                    if (File.Exists(directoryInfo.FullName + "/" + i + ".png")) {
                        BitmapImage image = new BitmapImage();

                        using (var stream = File.OpenRead(directoryInfo.FullName + "/" + i + ".png")) {
                            image.BeginInit();
                            image.CacheOption = BitmapCacheOption.OnLoad;
                            image.StreamSource = stream;
                            image.EndInit();
                        }

                        bitmapImages.Add(image);
                    }
                });
            }

            Task.Run(() => {
                int interval = 150;
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);

                while (true) {
                    for (int i = 0; i < bitmapImages.Count; i++) {
                        this.Dispatcher.Invoke(() => {
                            main.Source = bitmapImages[i];
                        });
                        Task.Delay(interval).Wait();
                    }
                }
            }); // Animation
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {

            if (e.ChangedButton == MouseButton.Left)
                if (!pinned)
                    this.DragMove();
        }
        private void MenuItem_Checked(object sender, RoutedEventArgs e) {
            pinned = true;
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e) {
            pinned = false;
        }
        private void quitMenu_Click(object sender, RoutedEventArgs e) {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            this.Close();
            _previous.deleteItem(accessorie);
        }

        private void Window_Deactivated(object sender, EventArgs e) {
            /*Window window = sender as Window;
            _previous.Topmost = false;
            _previous.Topmost = true;
            _previous.Activate();
            window.Topmost = false; // set topmost false first
            window.Topmost = true;
            window.Activate();*/
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e) {


            if (e.Delta > 0)
                accessorie.Size += 10;
            else if (e.Delta < 0)
                accessorie.Size -= 10;
            this.Height = accessorie.Size;
            this.Width = accessorie.Size;
        }
    }
}
