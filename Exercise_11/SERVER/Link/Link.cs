using System;
using System.IO.Ports;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Threading;

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

        int BUFSIZE;

        /// <summary>
        /// Initializes a new instance of the <see cref="link"/> class.
        /// </summary>
        public Link(int BUFSIZE, string APP)
        {
            // Create a new SerialPort object with default settings.
            this.BUFSIZE = BUFSIZE;
#if DEBUG
            if (APP.Equals("FILE_SERVER"))
            {
                serialPort = new SerialPort("/dev/ttyS1", 115200, Parity.None, 8, StopBits.One);
                serialPort.ReadTimeout = 5000;
                serialPort.WriteBufferSize = BUFSIZE * 3;
                serialPort.ReadBufferSize = BUFSIZE * 3;
            }
            else
            {
                serialPort = new SerialPort("/dev/ttyS1", 115200, Parity.None, 8, StopBits.One);
                serialPort.ReadTimeout = 5000;
                serialPort.WriteBufferSize = BUFSIZE * 3;
                serialPort.ReadBufferSize = BUFSIZE * 3;
            }
#else
                serialPort = new SerialPort("/dev/ttyS1",115200,Parity.None,8,StopBits.One);
#endif
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }

            _buffer = new byte[(BUFSIZE * 2)];

            // Uncomment the next line to use timeout
            //serialPort.ReadTimeout = 500;
            serialPort.DiscardOutBuffer();
            serialPort.DiscardInBuffer();

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
        public void send(byte[] buf, int size)
        {
            string stringreceived = Encoding.ASCII.GetString(buf);
            var list_Buffer = new List<byte>(buf);

            for (int i = 0; i < list_Buffer.Count; i++)
            {
                if (list_Buffer[i] == (byte)'A')
                {
                    list_Buffer.RemoveAt(i);
                    list_Buffer.Insert(i, (byte)'B');
                    list_Buffer.Insert(i + 1, (byte)'C');
                    var slet = Encoding.ASCII.GetString(list_Buffer.ToArray());
                }
                else if (list_Buffer[i] == (byte)'B')
                {
                    list_Buffer.Insert(i + 1, (byte)'D');
                    var slet = Encoding.ASCII.GetString(list_Buffer.ToArray());
                }
            }

            list_Buffer.Insert(0, (byte)DELIMITER);
            list_Buffer.Insert(list_Buffer.Count, (byte)DELIMITER);

            buf = list_Buffer.ToArray();


            //buf = Encoding.ASCII.GetBytes((DELIMITER + stringreceived + DELIMITER));


            if (!serialPort.IsOpen)
                serialPort.Open();

            serialPort.Write(buf, 0, buf.Length);

            serialPort.DiscardInBuffer();
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
            var receiveLinkBuffer = new byte[BUFSIZE * 2];
            // TO DO Your own code           
            Array.Clear(receiveLinkBuffer, 0, receiveLinkBuffer.Length);
            bool firstRound = true;
            int bytesread = 0;

            while (serialPort.BytesToRead != 0 || firstRound == true)
            {
                try
                {
                    bytesread += serialPort.Read(receiveLinkBuffer, bytesread, receiveLinkBuffer.Length - bytesread);
                    Thread.Sleep(100);
                    firstRound = false;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
					Environment.Exit(1);
                }
            }

            var list_Buffer = new List<byte>(receiveLinkBuffer);
            int delimiterFirst = 0;
            int delimiterLast = 0;
            int counter = 0;

            if (((delimiterFirst = list_Buffer.FindIndex(str => str.Equals((byte)'A'))) != -1)
                && ((delimiterLast = list_Buffer.FindLastIndex(str => str.Equals((byte)'A'))) != -1))
            {
                if (delimiterFirst == delimiterLast)
                {
                    Console.WriteLine("No Delimiter was found");
                    Console.WriteLine($"The buffer cointained: {Encoding.ASCII.GetString(receiveLinkBuffer)}");
                    return -1;
                }
                list_Buffer.RemoveAll(str => str.Equals((byte)'A'));
                counter += 2;
                for (int i = delimiterFirst; i < delimiterLast; i++)
                {
                    if (list_Buffer[i].Equals((byte)'B') && list_Buffer[i + 1].Equals((byte)'D'))
                    {
                        list_Buffer.RemoveRange(i, 2);
                        list_Buffer.Insert(i, (byte)'B');
                        counter++;
                    }
                    if (list_Buffer[i].Equals((byte)'B') && list_Buffer[i + 1].Equals((byte)'C'))
                    {
                        list_Buffer.RemoveRange(i, 2);
                        list_Buffer.Insert(i, (byte)'A');
                        counter++;
                    }
                }

            }
            else
            {
                Console.WriteLine("No Delimiter was found");
                Console.WriteLine($"The buffer cointained: {Encoding.ASCII.GetString(receiveLinkBuffer)}");
                return -1;
            }
            list_Buffer.RemoveRange(delimiterLast + 1 - counter, list_Buffer.Count - delimiterLast - 1 + counter);
            buf = list_Buffer.ToArray();

            serialPort.DiscardOutBuffer();

            return buf.Length;
        }

    }
}
