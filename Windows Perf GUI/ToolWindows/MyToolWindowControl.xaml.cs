using System.Windows;
using System.Windows.Controls;

namespace Windows_Perf_GUI
{
    public partial class MyToolWindowControl : UserControl
    {
        public MyToolWindowControl()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            VS.MessageBox.Show("Windows_Perf_GUI", "Button clicked");
        }
    }
}