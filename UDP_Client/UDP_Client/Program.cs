using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDP_Client
{
    class Program
    {
        static void Main(string[] args)
        {
            
            

            var port = int.Parse(args[0]);
            var address = args[1];
            var command = args[2].ToLower();


            Console.WriteLine(command);
            if (command != 'u'.ToString() && command != 'l'.ToString())
            {
                Console.WriteLine("Incorrect input");
                return;
            }
            
            
            
            var client = new UdpClient(port);
            

            try
            {
                client.Connect(address, port);

                Console.WriteLine("Connected to " + address + " On port " + port);


                var sendBytes = Encoding.ASCII.GetBytes(args[2]);
                
                IPEndPoint remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                
                var receiveBytes = client.Receive(ref remoteIpEndPoint); 
                string returnData = Encoding.ASCII.GetString(receiveBytes);

                Console.WriteLine(returnData);
               
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());

            }

            
            
        }
    }
}