using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace tcp
{
	class file_server
	{
		/// <summary>
		/// The PORT
		/// </summary>
		const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		const int BUFSIZE = 1000;
        

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// Opretter en socket.
		/// Venter på en connect fra en klient.
		/// Modtager filnavn
		/// Finder filstørrelsen
		/// Kalder metoden sendFile
		/// Lukker socketen og programmet
 		/// </summary>
		private file_server ()
		{
			TcpListener serverSocket = new TcpListener(PORT);
            TcpClient clientSocket = default(TcpClient);
			RequestCount = 0; 
			serverSocket.Start();
			while (true)
            {
			//Wait till client & tcp connection has been established
			Console.WriteLine("Waiting for client requests"); 
			clientSocket = serverSocket.AcceptTcpClient();
			Console.WriteLine("Connection from client has been accepted");
            
			//Infinite loop for monitoring requests from Clients
            
				//Reads data from NetworkSteam (data from Client) 
                //Add client to request count
			    RequestCount = RequestCount + 1;
                //Establish network stream connection
                NetworkStream networkStream = clientSocket.GetStream();
               
				//Get filename from client
				string dataFromClient = LIB.readTextTCP(networkStream);
				//Size of file 
				Byte[] bytesReceived = Encoding.ASCII.GetBytes(dataFromClient);

				Console.WriteLine($"File requested from client: {dataFromClient}");
                
				sendFile(dataFromClient, BitConverter.ToInt32(bytesReceived, 0), networkStream); 

            }
			clientSocket.Close();
			serverSocket.Stop();
			Console.WriteLine("Server has been shut down");
			Console.ReadLine(); 
		}

		public int RequestCount { get; set; }

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// The filename.
		/// </param>
		/// <param name='fileSize'>
		/// The filesize.
		/// </param>
		/// <param name='io'>
		/// Network stream for writing to the client.
		/// </param>
		private void sendFile (String fileName, long fileSize, NetworkStream io)
		{
			string serverResponse; 
			Byte[] sendBytes; 

			if(LIB.check_File_Exists(fileName) != 0)
			{
				Console.WriteLine($"Trying to send file: {fileName}");
				FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                
				long sizeOfFile = LIB.check_File_Exists(fileName);
				LIB.writeTextTCP(io, sizeOfFile.ToString());
				io.Flush();

				sendBytes = new byte[BUFSIZE];
				int readBytes = 0;
				while ((readBytes = fileStream.Read(sendBytes, 0, BUFSIZE)) > 0)
				{
					serverResponse = Encoding.ASCII.GetString(sendBytes);
					LIB.writeTextTCP(io, serverResponse);
				}
                io.Flush();                 
			}
			else
			{
				Console.WriteLine($"The file {LIB.extractFileName(fileName)} did not exist");
				//File was not found, send errormessage 
                //MANGLER, VIRKER IKKE
                int err = 0;
                LIB.writeTextTCP(io, err.ToString());
                io.Flush();

			}
            //Flush data from stream 

		}

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Server starts...");
			var serverSocket = new file_server();          

          }
	}
}
