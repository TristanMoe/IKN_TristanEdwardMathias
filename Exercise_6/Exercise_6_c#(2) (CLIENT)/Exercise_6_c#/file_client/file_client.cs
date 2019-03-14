using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text; 

namespace tcp
{
	class file_client
	{
		/// <summary>
		/// The PORT.
		/// </summary>
		const int PORT = 9000;
		/// <summary>
		/// The BUFSIZE.
		/// </summary>
		const int BUFSIZE = 1000;
        
		public static string Desiredfile;


		/// <summary>
		/// Initializes a new instance of the <see cref="file_client"/> class.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments. First ip-adress of the server. Second the filename
		/// </param>
		private file_client(string[] args)
		{
			TcpClient clientSocket = new TcpClient();
			clientSocket.Connect(args[0], PORT);
			Console.WriteLine("Client connected to server");

			Desiredfile = args[1];

  			NetworkStream serverStream = clientSocket.GetStream();

			LIB.writeTextTCP(serverStream, Desiredfile);

            //byte[] outStream = System.Text.Encoding.ASCII.GetBytes(Desiredfile + "$");
            //serverStream.Write(outStream, 0, outStream.Length);
            //serverStream.Flush();

			receiveFile(Desiredfile, serverStream);
			clientSocket.Close();
		}

		/// <summary>
		/// Receives the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='io'>
		/// Network stream for reading from the server
		/// </param>
		private void receiveFile (String fileName, NetworkStream io)
		{
			long filesize = LIB.getFileSizeTCP(io);
			Byte[] bytesReceived;
			string ReceiveData = ""; 
			int bytesRead = 0; 

            if(filesize == 0)
			{
				Console.WriteLine("File did not exist, try again");
				return; 
			}
           
			Console.WriteLine($"File {LIB.extractFileName(fileName)} has been recieved");
			Console.WriteLine($"Filesize: {filesize.ToString()}");
			bytesReceived = new byte[BUFSIZE];

            while (bytesRead < filesize)
			{
				bytesRead += io.Read(bytesReceived, 0, BUFSIZE);
				ReceiveData += Encoding.ASCII.GetString(bytesReceived);
			}      
            //SaveFile(ReceiveData);
			Console.WriteLine($"Data received: {ReceiveData}");
          
        }

        private void SaveFile(String filedata)
		{
			string path = "/home/ikn/Desktop/TextReceived.txt";
			TextWriter tw = new StreamWriter(path,true);
			tw.Write($"{filedata}");           
			tw.Close();
		}

        
		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			Console.WriteLine ("Client starts...");
			new file_client(args);
			Console.ReadKey();
		}
	}
}
