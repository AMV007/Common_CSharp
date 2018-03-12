using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace CommonControls.CommonMethods.Net
{
    public class c_WebPage
    {
        public static string Login(string WebAddress)
        {
            Uri CurrentDownloadUri = new Uri(WebAddress);

            IPHostEntry hostEntry = Dns.GetHostEntry(CurrentDownloadUri.Host);
            IPAddress address = hostEntry.AddressList[0];
            IPEndPoint ipe = new IPEndPoint(address, 80);
            Socket socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);            

            try
            {
                socket.Connect(ipe);
                if (socket.Connected)
                {
                    Console.WriteLine("Connected to " + ipe.ToString());
                }
                else
                {
                    Console.WriteLine("Can not connect...");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }

            string request =
             "POST " + CurrentDownloadUri.AbsolutePath + " HTTP/1.1\r\n" +
             "Accept: text/html\r\n" +
             "User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)\r\n" +
             "Host: " + hostEntry.HostName + "\r\n" +
             "Cookie: income=1\r\n" +
             "Referer: " + WebAddress + "\r\n" +
             "Content-Length: " + "142" + "\r\n" +
             "Content-Type: application/x-www-form-urlencoded\r\n\r\n"
             + ""; // параметры

            Byte[] bytesSent = System.Text.Encoding.ASCII.GetBytes(request);
            Byte[] bytesReceived = new Byte[1024];
            socket.Send(bytesSent, bytesSent.Length, 0);
            string page = "";
            int bytes = 0;

            do
            {
                bytes = socket.Receive(bytesReceived, bytesReceived.Length, 0);
                page = page + System.Text.Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            }
            while (bytes > 0);

            socket.Close();
            return page;
        }

        public static string Download(string WebAddress, bool AcceptGZIP)
        {
            Uri CurrentDownloadUri = new Uri(WebAddress);

            IPHostEntry hostEntry = Dns.GetHostEntry(CurrentDownloadUri.Host);
            IPAddress address = hostEntry.AddressList[0];
            IPEndPoint ipe = new IPEndPoint(address, 80);
            Socket socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                socket.Connect(ipe);
                if (socket.Connected)
                {
                    Console.WriteLine("Connected to " + ipe.ToString());
                }
                else
                {
                    Console.WriteLine("Can not connect...");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.Message);
            }

            string request =
                "GET " + CurrentDownloadUri.AbsolutePath + " HTTP/1.1\r\n";
            if (AcceptGZIP)
            {
                request += "Accept-Encoding: gzip,deflate\r\n";
            }
            else
            {
                request += "Accept-Encoding: deflate\r\n";
            }
            request += "User-Agent: Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1; SV1)\r\n"+
                "Host: " + hostEntry.HostName + "\r\n" +
                "Referer: " + WebAddress + "\r\n" +
                "Connection: Close\r\n\r\n";

            /* "Accept: text/html\r\n" +
             "Host: " + hostEntry.HostName + "\r\n" +
            // "Cookie: income=1\r\n" +
             "Referer: " + WebAddress + "\r\n" +
             //"Content-Length: " + "142" + "\r\n" +
             //"Content-Type: application/x-www-form-urlencoded\r\n\r\n"
              ""; // параметры*/

            Byte[] bytesSent = System.Text.Encoding.ASCII.GetBytes(request);
            Byte[] bytesReceived = new Byte[512];
            socket.Send(bytesSent, bytesSent.Length, 0);
            string page = "";
            int bytes = 0;

            do
            {
                bytes = socket.Receive(bytesReceived);
               /* for (int i = 0; i < bytes; i++)
                {
                    page += (char)bytesReceived[i];
                }*/
                page = page + System.Text.Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            }
            while (bytes > 0);

            socket.Close();
            return page;
        }
    }
}
