using CmlLib.Core.Auth;
using CmlLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CmlLib.Core;
using CmlLib.Core.Auth.Microsoft;
using CmlLib.Core.Auth.Microsoft.UI.Wpf;
using System.Net;
using System.IO.Compression;
using System.IO;
using System.Windows.Media.Animation;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const double DefaultWidth = 295;
        private const double DefaultHeight = 76;
        private const double TargetWidth = 320;
        private const double TargetHeight = 85;
        static string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
        public MainWindow()
        {
            InitializeComponent();
            IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
            var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
            var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
            DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));

            this.MouseLeftButtonDown += MainWindows_MouseLeftButtonDown;
            CloseButton.MouseLeftButtonDown += Close;
            Play.MouseEnter += MyButton_MouseEnter;
            Play.MouseLeave += MyButton_MouseLeave;
        }
        public enum DWMWINDOWATTRIBUTE
        {
            DWMWA_WINDOW_CORNER_PREFERENCE = 33
        }
        public enum DWM_WINDOW_CORNER_PREFERENCE
        {
            DWMWCP_DEFAULT = 0,
            DWMWCP_DONOTROUND = 1,
            DWMWCP_ROUND = 2,
            DWMWCP_ROUNDSMALL = 3
        }

        [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        internal static extern void DwmSetWindowAttribute(IntPtr hwnd,
                                                         DWMWINDOWATTRIBUTE attribute,
                                                         ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                         uint cbAttribute);
        void MainWindows_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        async void StartBRC()
        {
            try
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = 256;

                var path = new MinecraftPath();

                var launcher = new CMLauncher(path);
                var launchOption = new MLaunchOption
                {
                    MaximumRamMb = 1024,
                    Session = MSession.GetOfflineSession("hello"),
                };

                var process = await launcher.CreateProcessAsync("BRC", launchOption);
                process.Start();
            }
            catch(Exception e)
            {
                MessageBox.Show("에러 발생 catch문 진입:" + e);
                string minecraftVersionsPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    ".minecraft",
                    "versions"
                );

                // 다운로드할 파일 URL
                string url = "https://github.com/ButterackingClient/ButterackingClient/releases/download/main/Butteracking.Client.zip";

                // 다운로드할 파일 경로 및 이름
                string downloadPath = System.IO.Path.Combine(minecraftVersionsPath, "Butteracking.Client.zip");

                // 압축을 풀 폴더 경로
                string extractFolderPath = System.IO.Path.Combine(minecraftVersionsPath, "Butteracking.Client");

                // 폴더 생성
                Directory.CreateDirectory(minecraftVersionsPath);

                // 파일 다운로드
                WebClient webClient = new WebClient();
                webClient.DownloadFile(url, downloadPath);

                // 압축 풀기
                ZipFile.ExtractToDirectory(downloadPath, extractFolderPath);

                // BRC 폴더 옮기기
                string brcFolderPath = System.IO.Path.Combine(extractFolderPath, "BRC");
                string destinationFolderPath = System.IO.Path.Combine(minecraftVersionsPath, "BRC");
                Directory.Move(brcFolderPath, destinationFolderPath);

                // 다운로드한 zip 파일 삭제
                File.Delete(downloadPath);

                Console.WriteLine("압축 해제가 완료되었습니다.");
            }
        }

        async private void Button_Click(object sender, RoutedEventArgs e)
        {
                StartBRC();
        }
        void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void MyButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AnimateButtonSize(TargetWidth, TargetHeight, 1);
            AnimateButtonColor("#FF4509A0", 0.5);
        }

        private void MyButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            AnimateButtonSize(DefaultWidth, DefaultHeight, 1);
            AnimateButtonColor("#FF6B0DF9", 0.5);
        }

        private void AnimateButtonSize(double targetWidth, double targetHeight, double durationSeconds)
        {
            // Animation code for resizing the button
        }

        private void AnimateButtonColor(string targetColor, double durationSeconds)
        {
            ColorAnimation colorAnimation = new ColorAnimation
            {
                To = (Color)ColorConverter.ConvertFromString(targetColor),
                Duration = TimeSpan.FromSeconds(durationSeconds),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
            };

            Storyboard storyboard = new Storyboard();
            storyboard.Children.Add(colorAnimation);

            Storyboard.SetTarget(storyboard, Play);
            Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));

            storyboard.Begin(this);
        }
    }
}
