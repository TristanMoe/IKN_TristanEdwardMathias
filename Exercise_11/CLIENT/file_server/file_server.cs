using System;
using System.IO;
using System.Text;
using Transportlaget;
using Library;

namespace Application
{
	class file_server
	{
		/// <summary>
		/// The BUFSIZE
		/// </summary>
		private const int BUFSIZE = 1000;
		private const string APP = "FILE_SERVER";
		private byte[] fileNameBuffer = new byte[BUFSIZE]; 
       

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
			// TO DO Your own code
			var tpl = new Transport(BUFSIZE, APP);
			string filenameRequest;

            while(true)
			{
				Console.WriteLine("Waiting for client requests");
				tpl.receive(ref fileNameBuffer);
				filenameRequest = Encoding.ASCII.GetString(fileNameBuffer);
				Console.WriteLine($"Trying to send file {filenameRequest}");
				sendFile(filenameRequest, 0, tpl);
				Console.WriteLine("File has been sent"); 
                
			}
           
		}

		/// <summary>
		/// Sends the file.
		/// </summary>
		/// <param name='fileName'>
		/// File name.
		/// </param>
		/// <param name='fileSize'>
		/// File size.
		/// </param>
		/// <param name='tl'>
		/// Tl.
		/// </param>
		private void sendFile(String fileName, long fileSize, Transport transport)
		{
			// TO DO Your own code 
			FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
			long sizeOfFile = 0; 
			if((sizeOfFile = LIB.check_File_Exists(fileName)) == 0)
			{
				var fileDidNotExistBytes = Encoding.ASCII.GetBytes("0");
				transport.send(fileDidNotExistBytes, fileDidNotExistBytes.Length);
				return; 
			}
            
			int readBytes = 0;
            Byte[] sendBytes = new byte[BUFSIZE];

			//Send file size
			byte[] fileSizeByte = Encoding.ASCII.GetBytes(sizeOfFile.ToString());
			transport.send(fileSizeByte, fileSizeByte.Length); 

            //Clear buffer
			Array.Clear(sendBytes, 0, BUFSIZE);


            //Send entire file 
            while((readBytes = fileStream.Read(sendBytes, 0, BUFSIZE)) > 0)
			{
				transport.send(sendBytes, BUFSIZE);

                //Clear buffer
				Array.Clear(sendBytes, 0, BUFSIZE); 
			}                   
        }

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name='args'>
		/// The command-line arguments.
		/// </param>
		public static void Main (string[] args)
		{
			var server = new file_server(); 
		}
	}
}