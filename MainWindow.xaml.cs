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
        static bool GameStarted = false;

        static FileInfo op_file = new FileInfo("C:\\Users\\" + username + "\\AppData\\Roaming\\brcl\\option.txt");
        static DirectoryInfo brcl_file = new DirectoryInfo("C:\\Users\\" + username + "\\AppData\\Roaming\\brcl");
        public MainWindow()
        {
            InitializeComponent();

            Version win11Version = new Version(10, 0, 22000, 0);
            Version osVersion = Environment.OSVersion.Version;
            if (osVersion >= win11Version)
            {
                IntPtr hWnd = new WindowInteropHelper(GetWindow(this)).EnsureHandle();
                var attribute = DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE;
                var preference = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
                DwmSetWindowAttribute(hWnd, attribute, ref preference, sizeof(uint));

                [DllImport("dwmapi.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
                static extern void DwmSetWindowAttribute(IntPtr hwnd,
                                                 DWMWINDOWATTRIBUTE attribute,
                                                 ref DWM_WINDOW_CORNER_PREFERENCE pvAttribute,
                                                 uint cbAttribute);
            }

            this.MouseLeftButtonDown += MainWindows_MouseLeftButtonDown;
            this.Loaded += Load;

            CloseButton.MouseLeftButtonDown += Close;
            Play.MouseEnter += MyButton_MouseEnter;
            Play.MouseLeave += MyButton_MouseLeave;
            CloseButton.MouseEnter += CloseButton_MouseEnter;
            CloseButton.MouseLeave += CloseButton_MouseLeave;
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
        void MainWindows_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        async void StartBRC()
        {
            Play.IsEnabled = false;
            AnimateButtonColor("#FF0C001F", 0.5, Play);
            GameStarted = true;
            try
            {
                System.Net.ServicePointManager.DefaultConnectionLimit = 256;
                var loginHandler = JELoginHandlerBuilder.BuildDefault();
                var accounts = loginHandler.AccountManager.GetAccounts();
                var session = await loginHandler.Authenticate();

                var path = new MinecraftPath();

                string[] startData = File.ReadAllLines(op_file.FullName);
                var launcher = new CMLauncher(path);
                var launchOption = new MLaunchOption
                {
                    MaximumRamMb = Int32.Parse(startData[0]),
                    Session = session,
                    ScreenWidth = Int32.Parse(startData[1]),
                    ScreenHeight = Int32.Parse(startData[2]),
                };



                var process = await launcher.CreateProcessAsync("BRC", launchOption);
                process.Start();
            }
            catch(Exception e)
            {
                string minecraftVersionsPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    ".minecraft",
                    "versions"
                );

                // 다운로드할 파일 URL
                string url = "https://github.com/ButterackingClient/ButterackingClient/releases/download/untagged-019d37228ff2da8917d7/brclient.ziphttps://github.com/ButterackingClient/ButterackingClient/releases/download/untagged-019d37228ff2da8917d7/brclient.zip";

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
            Play.IsEnabled = true;
            AnimateButtonColor("#FF4509A0", 0.5, Play);
            GameStarted = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            StartBRC();
        }
        void Close(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CloseButton_MouseEnter(object sender, RoutedEventArgs e)
        {
            AnimateButtonColor("#f70000", 0.3, CloseButton);
        }
        void CloseButton_MouseLeave(object sender, RoutedEventArgs e)
        {
            AnimateButtonColor("#FF4E4E4E", 0.3, CloseButton);
        }

        private void MyButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(GameStarted == false)
            {
                AnimateButtonColor("#FF4509A0", 0.5, Play);
            }
        }

        private void MyButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if(GameStarted == false)
            {
                AnimateButtonColor("#FF6B0DF9", 0.5, Play);
            }
        }

        private void AnimateButtonColor(string targetColor, double durationSeconds, DependencyObject a)
        {
            try
            {
                ColorAnimation colorAnimation = new ColorAnimation
                {
                    To = (Color)ColorConverter.ConvertFromString(targetColor),
                    Duration = TimeSpan.FromSeconds(durationSeconds),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(colorAnimation);

                Storyboard.SetTarget(storyboard, a);
                Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Button.Background).(SolidColorBrush.Color)"));


                storyboard.Begin(this);
            } catch
            {
                ColorAnimation colorAnimation = new ColorAnimation
                {
                    To = (Color)ColorConverter.ConvertFromString(targetColor),
                    Duration = TimeSpan.FromSeconds(durationSeconds),
                    EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseInOut }
                };

                Storyboard storyboard = new Storyboard();
                storyboard.Children.Add(colorAnimation);

                Storyboard.SetTarget(storyboard, a);
                Storyboard.SetTargetProperty(colorAnimation, new PropertyPath("(Rectangle.Fill).(SolidColorBrush.Color)"));


                storyboard.Begin(this);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Page1 page1 = new Page1();
            Panel.Navigate(page1);
        }
        
        void Load(object sender, RoutedEventArgs e)
        {
            Page2 page2 = new Page2();
            Panel.Navigate(page2);

            if (brcl_file.Exists == false)
            {
                Directory.CreateDirectory(brcl_file.FullName);
            }
            if (op_file.Exists == false)
            {
                File.WriteAllText(op_file.FullName, "6000" + "\n" + "1920" + "\n" +"1080");
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Page2 page2 = new Page2();
            Panel.Navigate(page2);
        }
    }
}
