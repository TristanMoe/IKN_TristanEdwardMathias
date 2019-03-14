using System;
namespace Exercise7_UDPServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
			var UDP_listener = new UDP_Client_Listener();
			UDP_listener.StartUDPServer(); 
            
        }
    }
}
