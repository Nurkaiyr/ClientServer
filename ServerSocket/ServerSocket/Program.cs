using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServerSocket
{
    class Program
    {
        public static List<Client> clients = new List<Client>();

        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 3535;
            IPEndPoint endPoint = new IPEndPoint(ip, port);

            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Bind(endPoint);
            serverSocket.Listen(3);

            Console.WriteLine("Server is listening...");

            Socket clientSocket = null;
            while (true)
            {
                clientSocket = serverSocket.Accept();

                Console.WriteLine("Someone send message");

                Task task = new Task(() => User(clientSocket));
                task.Start();
            }

            Task.Run(() =>
            {
                while (true)
                {
                    byte[] buf = new byte[1024];
                    StringBuilder builder = new StringBuilder();
                    do
                    {
                        int num = clientSocket.Receive(buf);
                        builder.Append(Encoding.Default.GetString(buf), 0, num);
                    }
                    while (clientSocket.Available > 0);
                }
            });
        }

        private static void User(Socket clientSocket)
        {
            while (true)
            {
                //получение данных message
                byte[] buf = new byte[1024];
                StringBuilder builder = new StringBuilder();

                do
                {
                    int num = clientSocket.Receive(buf);
                    builder.Append(Encoding.Default.GetString(buf), 0, num);
                }
                while (clientSocket.Available > 0);
                string json = builder.ToString();
                Message message = JsonConvert.DeserializeObject<Message>(json);

                Client client = clients.FirstOrDefault(c => c.Name == message.From);
                if (client == null)
                {
                    clients.Add(new Client() { Name = message.From, Socket = clientSocket });
                }
                BroadcastMessage(json);
            }
        }

        private static void BroadcastMessage(string json)
        {
            foreach (var item in clients)
            {
                item.Socket.Send(Encoding.Default.GetBytes(json), 0, Encoding.Default.GetBytes(json).Length, SocketFlags.None);
            }
        }
    }
}
