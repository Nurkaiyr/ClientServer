using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace clientSocket
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 3535;
            IPEndPoint endPoint = new IPEndPoint(ip, port);

            //отправка сообщ
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            serverSocket.Connect(endPoint);

            byte[] buf = Encoding.Default.GetBytes("some message");
            serverSocket.Send(buf);

            //прием ответа
            byte[] buf2 = new byte[1024];
            StringBuilder builder2 = new StringBuilder();

            do
            {
                int num = serverSocket.Receive(buf2);
                builder2.Append(Encoding.Default.GetString(buf2), 0, num);
            }
            while (serverSocket.Available > 0);

            Console.WriteLine("Ответ: " + builder2.ToString());
            Console.Read();
        }
    }
}
