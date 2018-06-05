using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using Newtonsoft.Json;

namespace clientSocketWpf
{
    public partial class MainWindow : Window
    {
        private IPAddress ip = IPAddress.Parse("127.0.0.1");
        private int port = 3535;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BtnSendClick(object sender, RoutedEventArgs e)
        {
            IPEndPoint endPoint = new IPEndPoint(ip, port);
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(endPoint);

            //отправка сообщ newMessage
            Message newMessage = new Message() { From = textBoxNik.Text, Text = txtBoxMyMsg.Text };
            string json = JsonConvert.SerializeObject(newMessage);
            byte[] buf = Encoding.Default.GetBytes(json);
            serverSocket.Send(buf, 0, buf.Length, SocketFlags.None);

            Task task = new Task(() => ReceiveMessage(serverSocket));
            task.Start();

            txtBoxMyMsg.Text = "";
        }

        private void ReceiveMessage(Socket serverSocket)
        {
            while (true)
            {
                try
                {
                    //получение
                    byte[] data = new byte[1024]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = serverSocket.Receive(data);
                        builder.Append(Encoding.Default.GetString(data, 0, bytes));
                    }
                    while (serverSocket.Available > 0);

                    string json = builder.ToString();
                    Message messageResp = JsonConvert.DeserializeObject<Message>(json);

                    Dispatcher.Invoke(() => { txtBoxChat.Text += messageResp.From + ": " + messageResp.Text + Environment.NewLine; });
                }
                catch
                {
                    serverSocket.Close();
                }
            }
        }
    }
}
