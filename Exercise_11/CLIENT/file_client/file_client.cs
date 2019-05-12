using System;
using System.IO;
using System.Text;
using Transportlaget;
using Linklaget; 
using Library;
using System.Threading; 

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
		private void receiveFile(String fileName, Transport transport)
		{
			// TO DO Your own code
			byte[] receiveBuffer = new byte[BUFSIZE];
			string ReceivedData = "";
			long filesize = 0;

			//Get file size
			if ((filesize = transport.receive(ref receiveBuffer)) == 0)
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
		public static void Main(string[] args)
		{
			try
			{
				//For Application: 
				//new file_client(args);

				#region Transport Layer Test
				//For Transport Layer test
                
					var t_client = new Transport(BUFSIZE, APP);
					var bytesToReceive = new byte[BUFSIZE*10];
					t_client.receive(ref bytesToReceive);
				    Console.WriteLine(Encoding.ASCII.GetString(bytesToReceive)); 
                

                #endregion

				#region Link Layer Test
                /*
				//For link layer test
				var link_client = new Link(BUFSIZE, APP);
				Thread.Sleep(1000); 
				byte[] bytesToReceive = new byte[BUFSIZE];
				string textToSend = "Det Har Du Nok Ret I!";
				byte[] bytesToSend = Encoding.ASCII.GetBytes(textToSend); 
				while (true)
				{
					link_client.receive(ref bytesToReceive);
					Console.WriteLine(Encoding.ASCII.GetString(bytesToReceive));
					Thread.Sleep(2000);
					link_client.send(bytesToSend, bytesToSend.Length);                     
				}
                */
				#endregion

			}
			catch (IOException e)
			{
				Console.WriteLine(e.Message);
			}
			Console.ReadLine();
		}
	}
}