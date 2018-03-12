using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace CommonControls.CommonMethods.Net
{
    public class WebConn
    {
        public struct ConnectionInfoStruct
		{
			public Socket ConnectionSocket;
			public IPHostEntry HostInfo;
			public IPEndPoint EndPoint;

            public string HostName;
            public string[] IPAddresses;
		}
        ConnectionInfoStruct ConnectSocket(string hostname, int port, ProtocolType ThisProtocol)
        {
            ConnectionInfoStruct result = new ConnectionInfoStruct();
            IPHostEntry ipHostInfo = null;            

            ipHostInfo = Dns.GetHostEntry(hostname);
            result.HostName = ipHostInfo.HostName;
            result.IPAddresses = new string[ipHostInfo.AddressList.Length];

            for (int i=0;i<ipHostInfo.AddressList.Length;i++)
            {
                result.IPAddresses[i] = ipHostInfo.AddressList[i].ToString();
            }

            foreach (IPAddress currentIP in ipHostInfo.AddressList)
            {
                Console.WriteLine("Соединяюсь с " + currentIP);
                IPEndPoint Target = new IPEndPoint(currentIP, port);

                Socket tempSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ThisProtocol);

                tempSocket.Connect(Target);

                if (tempSocket.Connected)
                {
                    // Use the SelectWrite enumeration to obtain Socket status.
                    if (tempSocket.Poll(-1, SelectMode.SelectWrite))
                    {
                        Console.WriteLine("В этот сокет можно писать");
                    }
                    else if (tempSocket.Poll(-1, SelectMode.SelectRead))
                    {
                        Console.WriteLine("Этот сокет может только отсылать");
                    }
                    else if (tempSocket.Poll(-1, SelectMode.SelectError))
                    {
                       Console.WriteLine("В этом сокете ошибка");
                    }

                    result.ConnectionSocket = tempSocket;
                    result.HostInfo = ipHostInfo;
                    result.EndPoint = Target;

                    break;
                }                
            }               

            return result;
        }

        string SocketSendReceive(ConnectionInfoStruct ConnectionInfothis)
        {

            Socket s = ConnectionInfothis.ConnectionSocket;
            if (s == null)
            {
                Console.WriteLine("Connection failed");
                return"";
            }
            else
            {
                Console.WriteLine("Началась передача");
            }

            string request = "GET /inbox/ret23454 HTTP/1.1\r\n" +
                "Accept-Encoding: gzip,deflate\r\n" +
                "Host: " + ConnectionInfothis.EndPoint.Address.ToString() + "\r\n" +
                "Connection: Close\r\n\r\n";

            Byte[] bytesSent = System.Text.Encoding.ASCII.GetBytes(request);
            Byte[] bytesReceived = new Byte[256];


            // Send request to the server.
            s.Send(bytesSent, bytesSent.Length, 0);

            // Receive the server home page content.
            int bytes = 0;

            string page = "";
            // The following will block until te page is transmitted.
            do
            {
                bytes = s.Receive(bytesReceived);
                page += System.Text.Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            }
            while (bytes > 0);

            return page;
        }
    }
}
