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

namespace bradpad {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        public void mainButton_Clicked(object sender, RoutedEventArgs e) {
            this.mainCanvas.Visibility = System.Windows.Visibility.Visible;
            this.settingsCanvas.Visibility = System.Windows.Visibility.Hidden;
            //count++;
            //this.settingsButton.Content = count;
        }

        public void settingsButton_Clicked(object sender, RoutedEventArgs e) {
            this.mainCanvas.Visibility = System.Windows.Visibility.Hidden;
            this.settingsCanvas.Visibility = System.Windows.Visibility.Visible;
            //count2++;
            //this.mainButton.Content = count2;
        }
        
        public void f22Button_Clicked(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.SendKeys.SendWait("^t");
        }
    }
}
