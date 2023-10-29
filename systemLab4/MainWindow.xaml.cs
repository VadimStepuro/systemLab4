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

namespace systemLab4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            long totalTime = Int64.Parse( totalTimeBox.Text);
            int minTime = Int32.Parse(minTimeBox.Text);
            int maxTime = Int32.Parse(maxTimeBox.Text);
            Rectangle[] rectangles = {philosopher1, philosopher2, philosopher3, philosopher4, philosopher5 };
            Rectangle[] forksRectangles = {fork1, fork2, fork3, fork4, fork5};
            Philosopher philosopher = new Philosopher(minTime, maxTime, totalTime, Dispatcher, output, rectangles, forksRectangles);
            philosopher.Start();
            
        }
    }
}
