using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Windows.Media.Control;

namespace DesktopMe {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public string mainImg = AppDomain.CurrentDomain.BaseDirectory + "/images/body.png";
        public string main_Img = AppDomain.CurrentDomain.BaseDirectory + "/images/body_.png";
        public string eyeImg = AppDomain.CurrentDomain.BaseDirectory + "/images/eye1.png";
        public string eyeImg1 = AppDomain.CurrentDomain.BaseDirectory + "/images/eye2.png";
        public string mouthImg = AppDomain.CurrentDomain.BaseDirectory + "/images/mouth1.png";
        public string mouthImg1 = AppDomain.CurrentDomain.BaseDirectory + "/images/mouth.png";

        public string musicImg1 = AppDomain.CurrentDomain.BaseDirectory + "/images/effects/music1.png";
        public string musicImg2 = AppDomain.CurrentDomain.BaseDirectory + "/images/effects/music2.png";


        public string tailFolder = AppDomain.CurrentDomain.BaseDirectory + "/images/tail/";
        bool inDrag = false;
        Point anchorPoint;

        bool isPlayed = false;
        public MainWindow() {
            InitializeComponent();
            IniFile iniFile = new IniFile("file_name.ini");
            Width = iniFile.ReadInt("Width", "Size");
            Height = iniFile.ReadInt("Height", "Size");


            main.Source = new BitmapImage(new Uri(mainImg));
            eye.Source = new BitmapImage(new Uri(eyeImg));

            var waveIn = new NAudio.Wave.WaveInEvent {
                DeviceNumber = 0, // customize this to select your microphone device
                WaveFormat = new NAudio.Wave.WaveFormat(rate: 44100, bits: 16, channels: 1),
                BufferMilliseconds = 50
            };
            waveIn.DataAvailable += ShowPeakMono;
            waveIn.StartRecording();

            Random random = new Random();

            Task.Run(() => {
                var nextBlink = DateTime.Now + TimeSpan.FromSeconds(random.Next(5, 15));
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);

                while (true) {
                    if (DateTime.Now >= nextBlink) {
                        // Check if currently speaking, only blink if not in dialog

                        this.Dispatcher.Invoke(() => {
                            eye.Source = new BitmapImage(new Uri(eyeImg1));
                        });
                        Task.Delay(250).Wait();
                        this.Dispatcher.Invoke(() => {
                            eye.Source = new BitmapImage(new Uri(eyeImg));
                        });

                        nextBlink = DateTime.Now + TimeSpan.FromSeconds(random.Next(7, 15));
                    }
                }
            });
            Task.Run(() => {
                var nextBlink = DateTime.Now + TimeSpan.FromSeconds(0.5);
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);

                while (true) {
                    if (DateTime.Now >= nextBlink) {
                        // Check if currently speaking, only blink if not in dialog

                        this.Dispatcher.Invoke(() => {
                            main.Source = new BitmapImage(new Uri(mainImg));
                        });
                        Task.Delay(500).Wait();
                        this.Dispatcher.Invoke(() => {
                            main.Source = new BitmapImage(new Uri(main_Img));
                        });

                        nextBlink = DateTime.Now + TimeSpan.FromSeconds(0.5);
                    }
                }
            });
            Task.Run(() => {
                int rare = 5;
                int interval = 150;
                var nextBlink = DateTime.Now + TimeSpan.FromMilliseconds(random.Next(rare, rare + 20));
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);

                while (true) {
                    if (DateTime.Now >= nextBlink) {
                        // Check if currently speaking, only blink if not in dialog
                        DirectoryInfo directoryInfo = new DirectoryInfo(tailFolder);
                        for (int i = 1; i <= directoryInfo.GetFiles().Length; i++) {
                            this.Dispatcher.Invoke(() => {
                                tail.Source = new BitmapImage(new Uri(tailFolder + "/" + i + ".png"));
                            });
                            Task.Delay(interval).Wait();
                        }

                        nextBlink = DateTime.Now + TimeSpan.FromSeconds(random.Next(rare, rare + 20));
                    }
                }
            });
            Task.Run(() => {
                var nextBlink = DateTime.Now + TimeSpan.FromSeconds(1);
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);

                while (true) {

                    if (DateTime.Now >= nextBlink) {
                        // Check if currently speaking, only blink if not in dialog
                        if (isPlayed) {
                            this.Dispatcher.Invoke(() => {
                                music.Source = new BitmapImage(new Uri(musicImg1));
                            });
                            Task.Delay(500).Wait();
                            this.Dispatcher.Invoke(() => {
                                music.Source = new BitmapImage(new Uri(musicImg2));
                            });
                        }
                        else {
                            this.Dispatcher.Invoke(() => {
                                music.Source = null;
                            });
                        }
                        nextBlink = DateTime.Now + TimeSpan.FromSeconds(1);
                    }
                }
            });
            Task.Run(async () => {

                while (true) {
                    try {
                        var mediaTramsportManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                        var mediaSession = mediaTramsportManager.GetCurrentSession();
                        if(mediaSession != null) {
                            GlobalSystemMediaTransportControlsSessionPlaybackInfo playbackInfo = mediaSession.GetPlaybackInfo();

                            if (playbackInfo.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                                isPlayed = true;
                            else
                                isPlayed = false;

                            var mediaProperties = await mediaSession.TryGetMediaPropertiesAsync();

                            //Debug.WriteLine("{0} - {1}", mediaProperties.Title, mediaProperties);
                        }
                        else {
                            isPlayed = false;
                        }
                    }
                    catch (Exception ex) {

                    }
                    Task.Delay(250).Wait();
                }
            });
        }


        private void ShowPeakMono(object sender, NAudio.Wave.WaveInEventArgs args) {
            float maxValue = 32767;
            int peakValue = 0;
            int bytesPerSample = 2;
            for (int index = 0; index < args.BytesRecorded; index += bytesPerSample) {
                int value = BitConverter.ToInt16(args.Buffer, index);
                peakValue = Math.Max(peakValue, value);
            }

            if (peakValue > 1000) {
                this.Dispatcher.Invoke(() => {
                    mouth.Source = new BitmapImage(new Uri(mouthImg));

                });
            }
            else {
                this.Dispatcher.Invoke(() => {
                    mouth.Source = new BitmapImage(new Uri(mouthImg1));

                });
            }
        }



        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {

            if (e.ChangedButton == MouseButton.Left) {
            
            this.DragMove();
                /*anchorPoint = PointToScreen(e.GetPosition(this));
                inDrag = true;
                CaptureMouse();
                e.Handled = true;*/
            }
        }

        private void Window_MouseMove(object sender, MouseEventArgs e) {
            if (inDrag) {
                Point currentPoint = PointToScreen(e.GetPosition(this));
                this.Left = this.Left + currentPoint.X - anchorPoint.X;
                /*this.Top = this.Top + currentPoint.Y - anchorPoint.Y*/
            ; // this is not changing in your case
            anchorPoint = currentPoint;
            }
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {

            if (inDrag) {
                ReleaseMouseCapture();
                inDrag = false;
                e.Handled = true;
            }
        }

        private void Window_LostMouseCapture(object sender, MouseEventArgs e) {

            if (inDrag) {
                ReleaseMouseCapture();
                inDrag = false;
                e.Handled = true;
            }
        }

        private void settingsMenu_Click(object sender, RoutedEventArgs e) {
            SettingsForm settingsForm = new SettingsForm(this);

            settingsForm.ShowDialog();
        }

        private void quitMenu_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}