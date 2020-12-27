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
using System.Windows.Shapes;

namespace Packer {
    /// <summary>
    /// Interaktionslogik für Message.xaml
    /// </summary>
    public partial class Message : Window {
        public Message() {
            InitializeComponent();
        }

        public static void Show(string content, int fontSize = 18) {
            Show("Message", content);
        }

        public static void Show(string title, string content, int fontSize = 18) {
            Message m = new Message();
            m.Title = title;
            m.Mes.Text = content;
            m.Mes.FontSize = fontSize;
            m.Show(); // Show vom Objekt selber, also das öffnen des Fensters
        }

        private void Button_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Close();
        }
    }
}
