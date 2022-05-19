using System;
using System.Windows;

namespace SystemDependencies
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        VM vm = new VM();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void Openfile_Click(object sender, RoutedEventArgs e)
        {
            vm.RunFile();
        }

        private void Input_Click(object sender, RoutedEventArgs e)
        {
            vm.RunManuallyInput();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            vm.Clear();
        }
    }
}
