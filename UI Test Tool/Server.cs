using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UI_Test_Tool
{
    class Server
    {
        private TcpListener tcpListener;
        private Thread listenThread;
        public List<TcpClient> clientList;
        public bool sendmessage = true;

        public Server()
        {
            this.tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 1337);
            this.clientList = new List<TcpClient>();
            try
            {
                this.listenThread = new Thread(new ThreadStart(ListenForClients));
                this.listenThread.Start();
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                this.tcpListener.Stop();
            }
        }

        private void ListenForClients()
        {
            this.tcpListener.Start();

            while (true)
            {
                try
                {
                    TcpClient client = this.tcpListener.AcceptTcpClient();
                    this.clientList.Add(client);
                }
                catch (Exception e)
                {
                    // closed
                    Console.WriteLine(e.ToString());
                }
            }
        }

        public string getIP()
        {
            return ((IPEndPoint)this.tcpListener.LocalEndpoint).Address.ToString();
        }

        public void sendString(string message)
        {
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] buffer = encoder.GetBytes(message);
            List<TcpClient> clients = this.clientList.ToList();

            foreach (TcpClient client in clients)
            {
                NetworkStream clientStream = client.GetStream();
                try
                {
                    clientStream.Write(buffer, 0, buffer.Length);
                    clientStream.Flush();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Sendstring error:{0}", e);
                    clientStream.Close();
                    this.clientList.Remove(client);
                }
            }

        }

        public void close()
        {
            // Test with http://sockettest.sourceforge.net/
            this.tcpListener.Stop();
            foreach (TcpClient client in this.clientList)
            {
                client.GetStream().Close();
            }

            this.clientList.Clear();
        }
    }
}
