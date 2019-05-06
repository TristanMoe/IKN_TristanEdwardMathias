using System;
using System.IO;
using System.Text;
using Transportlaget;
using Linklaget; 
using Library;

namespace Application
{
	class file_client
	{
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_CLIENT";

		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// 
		/// file_client metoden opretter en peer-to-peer forbindelse
		/// Sender en forspÃ¸rgsel for en bestemt fil om denne findes pÃ¥ serveren
		/// Modtager filen hvis denne findes eller en besked om at den ikke findes (jvf. protokol beskrivelse)
		/// Lukker alle streams og den modtagede fil
		/// Udskriver en fejl-meddelelse hvis ikke antal argumenter er rigtige
		/// </summary>
		/// <param name='args'>
		/// Filnavn med evtuelle sti.
		/// </param>
	    private file_client(String[] args)
	    {
			var tpl = new Transport(BUFSIZE, APP); 
			// TO DO Your own code

			byte[] desiredfile = Encoding.ASCII.GetBytes(args[0]);
			Console.WriteLine($"Requesting file: {args[0]}");
			tpl.send(desiredfile, desiredfile.Length);
			Console.WriteLine($"Waiting for file from server");
            receiveFile(args[0], tpl); 
  	    }

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='transport'>
		/// Transportlaget
		/// </param>
		private void receiveFile (String fileName, Transport transport)
		{
			// TO DO Your own code
			byte[] receiveBuffer = new byte[BUFSIZE];
			string ReceivedData = "";
			long filesize = 0;

			//Get file size
			if((filesize = transport.receive(ref receiveBuffer)) == 0)
			{
				Console.WriteLine("File did not exist");
				return;                                
			}
			receiveBuffer = new byte[filesize];

			transport.receive(ref receiveBuffer);

			ReceivedData = Encoding.ASCII.GetString(receiveBuffer);
			Console.WriteLine($"DATA RECEIVED: \n {ReceivedData}");
		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// First argument: Filname
		/// </param>
		public static void Main (string[] args)
		{
			//new file_client(args);
			try{
				var link_Server = new Link(1000, "FILE_SERVER");

				//var link_Client = new Link(10000, "FILE_CLIENT"); 
                var buf = new byte[1000];


                //Server
                buf = Encoding.ASCII.GetBytes("Du skal huske At tage din hue Af, din Boev!");
                link_Server.send(buf, buf.Length);
                Array.Clear(buf, 0, buf.Length);
                Console.WriteLine("Done");

			}
            catch(Exception e)
			{
				Console.WriteLine(e.Message);
			}
            //Client
            //link_Client.receive(ref buf); 
            Console.ReadLine(); 
		}
	}
}