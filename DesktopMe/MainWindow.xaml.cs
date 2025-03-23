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
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Windows.ApplicationModel.Email;
using Windows.Media.Control;

namespace DesktopMe {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public static string character = "Just Guy";

        static string mainFolder = AppDomain.CurrentDomain.BaseDirectory + "/Characters/" + character;
        public string mainImg = mainFolder + "/body.png";
        public string main_Img = mainFolder + "/body_.png";
        public string eyeImg = mainFolder + "/eye1.png";
        public string eyeImg1 = mainFolder + "/eye2.png";
        public string mouthImg = mainFolder + "/mouth1.png";
        public string mouthImg1 = mainFolder + "/mouth.png";

        public string musicImg1 = mainFolder + "/effects/music1.png";
        public string musicImg2 = mainFolder + "/effects/music2.png";


        public string animFolder = mainFolder + "/anim/";
        bool inDrag = false;
        Point anchorPoint;

        bool isPlayed = false;
        IniFile iniFile;

        public int sensivity = 1000;

        public int eyeClosedTime = 250;
        public int eyeBlinkMin = 5;
        public int eyeBlinkMax = 15;

        bool pinned = false;

        void readData() {

            if (File.Exists("file_name.ini")) {

                iniFile = new IniFile("file_name.ini");
                Width = iniFile.ReadInt("Width", "Size");
                Height = iniFile.ReadInt("Height", "Size");

                Left = iniFile.ReadInt("X", "Position");
                Top = iniFile.ReadInt("Y", "Position");

                character = iniFile.ReadString("Character", "Other");
                sensivity = iniFile.ReadInt("Sensivity", "Other");

                eyeBlinkMin = iniFile.ReadInt("BlinkMin", "Other");
                eyeBlinkMax = iniFile.ReadInt("BlinkMax", "Other");
                eyeClosedTime = iniFile.ReadInt("BlinkTime", "Other");

                try {
                    pinned = bool.Parse(iniFile.ReadString("Pinned", "Other"));
                }
                catch(Exception e) {

                }
                
            }
            else {
                iniFile = new IniFile("file_name.ini");
            }

        }
        public MainWindow() {
            InitializeComponent();
            readData();
            pin.IsChecked = pinned;
            DirectoryInfo directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory + "/Characters/");
            foreach (DirectoryInfo fi in directoryInfo.GetDirectories()) {
                MenuItem menuItem = new MenuItem();
                menuItem.Header = fi.Name;
                menuItem.Click += (sender, e) => {
                    iniFile.Write("Character", fi.Name, "Other");
                    character = fi.Name;
                    mainFolder = AppDomain.CurrentDomain.BaseDirectory + "/Characters/" + character;
                    changeChar();
                    Debug.WriteLine(mainFolder);
                };
                charOption.Items.Add(menuItem);

            }

            mainFolder = AppDomain.CurrentDomain.BaseDirectory + "/Characters/" + character;
            if (Directory.Exists(mainFolder))
                changeChar();
            else {
                List<DirectoryInfo> list = directoryInfo.GetDirectories().ToList();
                character = list[new Random().Next(list.Count)].Name;
                changeChar();
            }
            

            var waveIn = new NAudio.Wave.WaveInEvent {
                DeviceNumber = 0, // customize this to select your microphone device
                WaveFormat = new NAudio.Wave.WaveFormat(rate: 44100, bits: 16, channels: 1),
                BufferMilliseconds = 50
            };
            waveIn.DataAvailable += ShowPeakMono;
            waveIn.StartRecording();

            Random random = new Random();

            Task.Run(() => {
                var nextBlink = DateTime.Now + TimeSpan.FromSeconds(random.Next(eyeBlinkMin, eyeBlinkMax));
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);

                while (true) {
                    if (DateTime.Now >= nextBlink) {
                        // Check if currently speaking, only blink if not in dialog

                        this.Dispatcher.Invoke(() => {
                            eye.Source = new BitmapImage(new Uri(eyeImg1));
                        });
                        Task.Delay(eyeClosedTime).Wait();
                        this.Dispatcher.Invoke(() => {
                            eye.Source = new BitmapImage(new Uri(eyeImg));
                        });

                        nextBlink = DateTime.Now + TimeSpan.FromSeconds(random.Next(eyeBlinkMin, eyeBlinkMax));
                    }
                }
            }); // Blink
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
            }); // Body
            Task.Run(() => {
                int rare = 5;
                int interval = 150;
                var nextBlink = DateTime.Now + TimeSpan.FromMilliseconds(random.Next(rare, rare + 20));
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Optimized);

                while (true) {
                    if (DateTime.Now >= nextBlink) {
                        // Check if currently speaking, only blink if not in dialog
                        DirectoryInfo directoryInfo = new DirectoryInfo(animFolder);
                        for (int i = 1; i <= directoryInfo.GetFiles().Length; i++) {
                            this.Dispatcher.Invoke(() => {
                                if(File.Exists(animFolder + "/" + i + ".png"))
                                    tail.Source = new BitmapImage(new Uri(animFolder + "/" + i + ".png"));
                            });
                            Task.Delay(interval).Wait();
                        }

                        nextBlink = DateTime.Now + TimeSpan.FromSeconds(random.Next(rare, rare + 20));
                    }
                }
            }); // Animation
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
                            Task.Delay(1000).Wait();
                            this.Dispatcher.Invoke(() => {
                                music.Source = new BitmapImage(new Uri(musicImg2));
                            });
                        }
                        else {
                            this.Dispatcher.Invoke(() => {
                                music.Source = null;
                            });
                        }
                        nextBlink = DateTime.Now + TimeSpan.FromMilliseconds(1000);
                    }
                }
            }); // Music Effect
            Task.Run(async () => {

                while (true) {
                    try {
                        var mediaTramsportManager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                        var mediaSession = mediaTramsportManager.GetCurrentSession();
                        if (mediaSession != null) {
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
            }); // Detect Music
        }
        void changeChar() {
            mainImg = mainFolder + "/body.png";
            main_Img = mainFolder + "/body_.png";
            eyeImg = mainFolder + "/eye1.png";
            eyeImg1 = mainFolder + "/eye2.png";
            mouthImg = mainFolder + "/mouth1.png";
            mouthImg1 = mainFolder + "/mouth.png";

            musicImg1 = mainFolder + "/effects/music1.png";
            musicImg2 = mainFolder + "/effects/music2.png";

            animFolder = mainFolder + "/anim/";

            main.Source = new BitmapImage(new Uri(mainImg));
            eye.Source = new BitmapImage(new Uri(eyeImg));
            mouth.Source = new BitmapImage(new Uri(mouthImg));

            if (File.Exists(animFolder + "/1.png"))
                tail.Source = new BitmapImage(new Uri(animFolder + "/1.png"));
            else
                tail.Source = null;

        }
        private void ShowPeakMono(object sender, NAudio.Wave.WaveInEventArgs args) {
            float maxValue = 32767;
            int peakValue = 0;
            int bytesPerSample = 2;
            for (int index = 0; index < args.BytesRecorded; index += bytesPerSample) {
                int value = BitConverter.ToInt16(args.Buffer, index);
                peakValue = Math.Max(peakValue, value);
            }

            if (peakValue > sensivity) {
                this.Dispatcher.Invoke(() => {
                    mouth.Source = new BitmapImage(new Uri(mouthImg));

                });
            }
            else {
                this.Dispatcher.Invoke(() => {
                    try {
                        mouth.Source = new BitmapImage(new Uri(mouthImg1));
                    }
                    catch (Exception ex) {
                    }

                });
            }
        }
        private void Window_MouseDown(object sender, MouseButtonEventArgs e) {

            if (e.ChangedButton == MouseButton.Left) {
                if(!pinned)
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

            if (settingsForm.ShowDialog() == true) {

                readData();
            }
        }
        private void quitMenu_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            iniFile.Write("X", Left.ToString(), "Position");
            iniFile.Write("Y", Top.ToString(), "Position");

            iniFile.Write("Pinned", pinned.ToString(), "Other");

        }

        private void MenuItem_Checked(object sender, RoutedEventArgs e) {
            pinned = true;
        }

        private void MenuItem_Unchecked(object sender, RoutedEventArgs e) {
            pinned = false;
        }

        private void Window_Deactivated(object sender, EventArgs e) {
            Window window = sender as Window;
            window.Topmost = true;
            window.Activate();
        }
    }
}