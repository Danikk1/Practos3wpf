using System.Collections.Generic;
using System.Windows;

namespace AudioPlayer
{
    public partial class Window2 : Window
    {
        public Window2(List<string> history)
        {
            InitializeComponent();
            historyListBox.ItemsSource = history;
        }
    }
}
