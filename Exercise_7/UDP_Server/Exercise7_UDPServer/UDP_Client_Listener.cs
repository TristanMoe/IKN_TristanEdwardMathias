using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

namespace Exercise7_UDPServer
{
    public class UDP_Client_Listener
    {
        const int PORT = 9000;
        const int BUFSIZE = 1000;
        IPAddress iP = new IPAddress(0x0A000001); //10.0.0.1
		const string uptime = "/proc/uptime";
		const string loadavg = "/proc/loadavg"; 


        public byte[] ReceiveBytes { get; set; }

        public void StartUDPServer()
        {
            UdpClient listener = new UdpClient(PORT);
            IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, PORT);
			string Request;
            
			try
			{
				while (true)
				{
					Console.WriteLine("Waiting for broadcast");
					ReceiveBytes = listener.Receive(ref groupEP);

					Console.WriteLine($"Received broadcast from: {groupEP}");
					Request = Encoding.ASCII.GetString(ReceiveBytes);
					Console.WriteLine($"Request received: {Request}");
					SendFile(Request, listener, groupEP); 
                    
                }
			}
            catch(SocketException e)
			{
			    Console.WriteLine(e); 
			}
			finally
			{
                listener.Close();
			}
            
        }

        public void SendFile(string type, UdpClient io, IPEndPoint groupEP)
		{
                         
			string typechosen; 
            switch (type)
			{
				case "u": { typechosen = uptime; } break;
				case "l": { typechosen = loadavg; } break;
				default:
					{
						Console.WriteLine("Invalid Request");
                        return; 
					}
      
			}
            
			Console.WriteLine($"Trying to send file: {typechosen} ");
            FileStream fileStream = new FileStream(typechosen, FileMode.Open,
                                                   FileAccess.Read);
			int readBytes;
			byte[] sendBytes = new byte[BUFSIZE]; 
            while((readBytes = fileStream.Read(sendBytes, 0, BUFSIZE)) > 0)
			{
				io.Send(sendBytes, readBytes, groupEP); 
			}            
        }
    }
}
