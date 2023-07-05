using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
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
using System.IO;

namespace WpfApp1
{
    /// <summary>
    /// Page1.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Page1 : Page
    {
        static string username = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split('\\')[1];
        static FileInfo op_file = new FileInfo("C:\\Users\\" + username + "\\AppData\\Roaming\\brcl\\option.txt");
        public Page1()
        {
            InitializeComponent();
            this.Loaded += Load;
        }

        private void Height_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            resolution.Content = Height.Value.ToString() + " x " + Width.Value.ToString();
        }

        private void Width_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            resolution.Content = Height.Value.ToString() + " x " + Width.Value.ToString();
        }

        void Load(object sender, RoutedEventArgs e)
        {
            string[] startData = File.ReadAllLines(op_file.FullName);
            RamSlider.Value = Int32.Parse(startData[0]);

            Height.Value = Int32.Parse(startData[1]);
            Width.Value = Int32.Parse(startData[2]);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(op_file.FullName, RamSlider.Value.ToString() + "\n" + Height.Value.ToString() + "\n" + Width.Value.ToString());
        }

        private void Ram_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            RamText.Content = RamSlider.Value.ToString() + "MB";
        }
    }
}
