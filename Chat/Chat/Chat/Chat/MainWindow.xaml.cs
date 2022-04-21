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
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;

namespace Chat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Socket socket = null;
        DispatcherTimer dTimer = null;

        public MainWindow()
        {
            InitializeComponent();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            IPAddress sender_address = IPAddress.Any;
            IPEndPoint sender_endpoint = new IPEndPoint(sender_address.MapToIPv4(), 65000);

            socket.Bind(sender_endpoint);
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            IPAddress receiver_address = IPAddress.Parse(txtIp.Text);
            IPEndPoint receiver_endpoint = new IPEndPoint(receiver_address, int.Parse(txtPorta.Text));

            byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text);

            socket.SendTo(messaggio, receiver_endpoint);

            dTimer = new DispatcherTimer();

            dTimer.Tick += new EventHandler(aggiornamento_dTimer);
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);
            dTimer.Start();
        }

        private void aggiornamento_dTimer(object sender, EventArgs e)
        {
            int bytes = 0;

            if ((bytes = socket.Available) > 0)
            {
                byte[] buffer = new byte[bytes];

                EndPoint receiverEndpoint = new IPEndPoint(IPAddress.Any, 0);

                bytes = socket.ReceiveFrom(buffer, ref receiverEndpoint);

                string from = ((IPEndPoint)receiverEndpoint).Address.ToString();
                string messaggio = Encoding.UTF8.GetString(buffer, 0, bytes);

                lst.Items.Add(from + ": " + messaggio);
            }
        }
    }
}
