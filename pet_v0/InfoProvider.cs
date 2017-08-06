using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Net.Sockets;
using System.Net;

namespace pet_v0
{
    public interface InfoProvider
    {
        void bindInfoWindow(InfoWindow infoWindow);
        void close();
    }


    public class UDPListener : InfoProvider
    {
        private InfoWindow infoWindow = null;
        private UdpClient udpServer;
        private bool continue_listening = true;
        private Thread thread = null;
        public const int PORT = 8846;

        public String providerName = "UDP Server [" + PORT + "]";


        private void listen()
        {
            try
            {
                IPEndPoint localIP = new IPEndPoint(IPAddress.Any, PORT);
                udpServer = new UdpClient(localIP);
            }
            catch (Exception)
            {
                udpServer = null;
                thread = null;
                infoWindow.OnInfoUpdated(providerName, "PORT BOUND FAILED");
                return;
            }

            while (continue_listening)
            {
                try
                {
                    IPEndPoint ip = new IPEndPoint(IPAddress.Any, 0);
                    byte[] buffer = udpServer.Receive(ref ip);
                    string str = Encoding.UTF8.GetString(buffer);
                    string ipaddress = "" + ip.Address.GetAddressBytes()[0];
                    ipaddress += "." + ip.Address.GetAddressBytes()[1];
                    ipaddress += "." + ip.Address.GetAddressBytes()[2];
                    ipaddress += "." + ip.Address.GetAddressBytes()[3];
                    if (continue_listening)
                    {
                        String text;
                        if (!ipaddress.Equals("127.0.0.1"))
                            text = "IP: " + ipaddress + "\nMessage: " + str;
                        else
                            text = str;
                        if (infoWindow != null)
                            infoWindow.OnInfoUpdated(providerName, text);

                    }
                }
                catch (Exception)
                {
                    //Console.WriteLine("closed");
                }
            }
            //udpServer.Close();
        }

        public UDPListener()
        {
            
        }


        void InfoProvider.bindInfoWindow(InfoWindow infoWindow)
        {
            this.infoWindow = infoWindow;
            thread = new Thread(listen);
            thread.Start();
            infoWindow.OnInfoUpdated(providerName, "Start collecting");
        }

        void InfoProvider.close()
        {
            if (thread == null) return;
            continue_listening = false;
            udpServer.Close();
            thread.Interrupt();
        }
    }

    public class FileListener : InfoProvider
    {
        private InfoWindow infoWindow = null;
        void InfoProvider.bindInfoWindow(InfoWindow infoWindow)
        {
            this.infoWindow = infoWindow;
        }

        void InfoProvider.close()
        {
            throw new NotImplementedException();
        }
    }

    public class TimerProvider: InfoProvider
    {
        private InfoWindow infoWindow = null;
        void InfoProvider.bindInfoWindow(InfoWindow infoWindow)
        {
            this.infoWindow = infoWindow;
        }

        void InfoProvider.close()
        {
            throw new NotImplementedException();
        }
    }
}
