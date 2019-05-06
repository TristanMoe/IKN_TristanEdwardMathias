using System;
using System.IO.Ports;
using System.Net;
using System.Text;

/// <summary>
/// Link.
/// </summary>
namespace Linklaget
{
	/// <summary>
	/// Link.
	/// </summary>
	public class Link
	{
		/// <summary>
		/// The DELIMITE for slip protocol.
		/// </summary>
		const char DELIMITER = 'A';
		/// <summary>
		/// The buffer for link.
		/// </summary>
		private byte[] _buffer;
		/// <summary>
		/// The serial port.
		/// </summary>
		SerialPort serialPort;

		/// <summary>
		/// Initializes a new instance of the <see cref="link"/> class.
		/// </summary>
		public Link (int BUFSIZE, string APP)
		{
			// Create a new SerialPort object with default settings.
			#if DEBUG
				if(APP.Equals("FILE_SERVER"))
				{
					serialPort = new SerialPort("/dev/ttyS0",115200,Parity.None,8,StopBits.One);
				}
				else
				{
					serialPort = new SerialPort("/dev/ttySn1",115200,Parity.None,8,StopBits.One);
				}
			#else
				serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
			#endif
			if(!serialPort.IsOpen)
				serialPort.Open();

			_buffer = new byte[(BUFSIZE*2)];

			// Uncomment the next line to use timeout
			//serialPort.ReadTimeout = 500;

			serialPort.DiscardInBuffer ();
			serialPort.DiscardOutBuffer ();
		}

		/// <summary>
		/// Send the specified buf and size.
		/// </summary>
		/// <param name='buf'>
		/// Buffer.
		/// </param>
		/// <param name='size'>
		/// Size.
		/// </param>
		public void send (byte[] buf, int size)
		{
			string stringreceived = System.Text.Encoding.Default.GetString(buf);

			string stringtosend = "";

			for (int i = 0; i < stringreceived.Length; i++)
			{
				if (stringreceived[i].Equals('A'))
				{
					stringtosend += "BC";
					size++;
				}

				if (stringreceived[i].Equals('B'))
				{
					stringtosend += "BD";
					size++;
				}
				else				
				    stringtosend += stringreceived[i];
			}
			buf = Encoding.ASCII.GetBytes(DELIMITER + stringtosend + DELIMITER); 
			
			if(!serialPort.IsOpen)
				serialPort.Open();

			Console.WriteLine(Encoding.ASCII.GetString(buf)); 
			
			serialPort.Write(buf,0,size);

			serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();
		}

        /// <summary>
        /// Receive the specified buf and size.
        /// </summary>
        /// <param name='buf'>
        /// Buffer.
        /// </param>
        /// <param name='size'>
        /// Size.
        /// </param>
        public int receive(ref byte[] buf)
        {
            //Encoding.ASCII.GetBytes("A"))
            // TO DO Your own code
            
            string output = "No Bytes Received";

           //Convert to string for tests and output 

			serialPort.Read(buf, 0, serialPort.ReadBufferSize);
            
            string read = Encoding.ASCII.GetString(buf);
            
            if (read.IndexOf(DELIMITER) > -1) 
            { 
	            
	            output = read.Substring(read.IndexOf(DELIMITER)+1, read.LastIndexOf(DELIMITER)-1); 
	            
	            Console.WriteLine(output); 
            }
            else
            {
	            Console.WriteLine(output);
	            return 0;
            }        
                         
            output.Replace("BC", "A");
            output.Replace("BD", "B");
            
            Console.WriteLine(output);

            buf = Encoding.ASCII.GetBytes(output);

			serialPort.DiscardInBuffer();
            serialPort.DiscardOutBuffer();

            return buf.Length; 
        }
    }
}
