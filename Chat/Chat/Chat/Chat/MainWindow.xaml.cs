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
        Socket socket = null; //Creazione di un oggetto della classe socket
        DispatcherTimer dTimer = null; //Creazione di un oggetto della classe DispatcherTimer.
                                       //Sia questo che l'oggetto socket li abbiamo creati dopo aver inserito gli using:
                                       //system.net, system.net.Sockets, system.windows.threading

        public MainWindow()
        {
            InitializeComponent();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); //Inizializzazione dell'oggetto socket, addressfamily specifica il piano di indirizzamento ed il tipo internetwork specifica l'ipv4
                                                                                                 //il tipo di socket dgram supporta messaggi senza connessione, non affidabili di lunghezza massima fissa
                                                                                                 //Il tipo di protocollo utilizzato è UDP

            IPAddress sender_address = IPAddress.Any; //Indirizzo Ip del mittente
            IPEndPoint sender_endpoint = new IPEndPoint(sender_address.MapToIPv4(), 65000); //La porta 65000 è stata scelta da noi

            socket.Bind(sender_endpoint);//Associamo un endpoint al socket
        }

        private void btnInvia_Click(object sender, RoutedEventArgs e)
        {
            IPAddress receiver_address = IPAddress.Parse(txtIp.Text); //Indirizzo ip del destinatario dedotto dalla textbox
            IPEndPoint receiver_endpoint = new IPEndPoint(receiver_address, int.Parse(txtPorta.Text)); //endpoint del destinatario dato dal suo ip e dal numero di porta dedotto dal textbox

            byte[] messaggio = Encoding.UTF8.GetBytes(txtMessaggio.Text); //creazione del messaggio da inviare, dedotto dalla textbox, e la sequenza di byte viene decodificata in strimga

            socket.SendTo(messaggio, receiver_endpoint); //metodo che dice di inviare certi dati ad uno specifico endpoint

            dTimer = new DispatcherTimer();

            dTimer.Tick += new EventHandler(aggiornamento_dTimer);//metodo che specifica l'azione da fare ad ogni tot di tempo specificato
            dTimer.Interval = new TimeSpan(0, 0, 0, 0, 250);//specifico il toto di tempo prima specificato
            dTimer.Start();

            txtMessaggio.Clear();
        }

        private void aggiornamento_dTimer(object sender, EventArgs e)
        {
            int bytes = 0;

            if ((bytes = socket.Available) > 0)//Verifico se ho ricveuto dei dati ogni 250 millisecondi (tot prima specificato)
            {
                byte[] buffer = new byte[bytes];//Ricezione dei dati

                EndPoint receiverEndpoint = new IPEndPoint(IPAddress.Any, 0); //Usiamo il parametro any perchè non conosciamo l'ip del mittente

                bytes = socket.ReceiveFrom(buffer, ref receiverEndpoint);

                string from = ((IPEndPoint)receiverEndpoint).Address.ToString(); //Recupero e salvo l'ip dell'entità mittente
                string messaggio = Encoding.UTF8.GetString(buffer, 0, bytes); //Trasformo i dati ricevuti in messaggio

                lst.Items.Add(from + ": " + messaggio);//Aggiungo alla listbox il messaggio ricevuto
            }
        }
    }
}
