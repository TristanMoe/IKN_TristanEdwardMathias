using System;
using Linklaget;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Transport.
/// </summary>
namespace Transportlaget
{
    /// <summary>
    /// Transport.
    /// </summary>
    public class Transport
    {
        /// <summary>
        /// The link.
        /// </summary>
        private Link link;
        /// <summary>
        /// The 1' complements checksum.
        /// </summary>
        private Checksum Checksum;
        /// <summary>
        /// The buffer.
        /// </summary>
        private byte[] buffer;
        /// <summary>
        /// The seq no.
        /// </summary>
        private byte seqNo;
        /// <summary>
        /// The old_seq no.
        /// </summary>
        private byte old_seqNo;
        /// <summary>
        /// The error count.
        /// </summary>
        private int errorCount;
        /// <summary>
        /// The DEFAULT_SEQNO.
        /// </summary>
        private const int DEFAULT_SEQNO = 2;
        /// <summary>
        /// The data received. True = received data in receiveAck, False = not received data in receiveAck
        /// </summary>
        private bool dataReceived;
        /// <summary>
        /// The number of data the recveived.
        /// </summary>
        private int recvSize = 0;
        private int BUFSIZE;

        /// <summary>
        /// Initializes a new instance of the <see cref="Transport"/> class.
        /// </summary>
        public Transport(int BUFSIZE, string APP)
        {
            this.BUFSIZE = BUFSIZE;
            link = new Link(BUFSIZE + (int)TransSize.ACKSIZE, APP);
            Checksum = new Checksum();
            buffer = new byte[BUFSIZE + (int)TransSize.ACKSIZE];
            seqNo = 0;
            old_seqNo = DEFAULT_SEQNO;
            errorCount = 0;
            dataReceived = false;
        }

        /// <summary>
        /// Receives the ack.
        /// </summary>
        /// <returns>
        /// The ack.
        /// </returns>
        private bool receiveAck()
        {
            recvSize = link.receive(ref buffer);
            dataReceived = true;

            if (recvSize == (int)TransSize.ACKSIZE)
            {
                dataReceived = false;
                if (!Checksum.checkChecksum(buffer, (int)TransSize.ACKSIZE) ||
                  buffer[(int)TransCHKSUM.SEQNO] != seqNo ||
                  buffer[(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
                {
                    return false;
                }
                seqNo = (byte)((buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
            }

            return true;
        }

        /// <summary>
        /// Sends the ack.
        /// </summary>
        /// <param name='ackType'>
        /// Ack type.
        /// </param>
        private void sendAck(bool ackType)
        {
            byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
            ackBuf[(int)TransCHKSUM.SEQNO] = (byte)
                (ackType ? (byte)buffer[(int)TransCHKSUM.SEQNO] : (byte)(buffer[(int)TransCHKSUM.SEQNO] + 1) % 2);
            ackBuf[(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
            Checksum.calcChecksum(ref ackBuf, (int)TransSize.ACKSIZE);
            link.send(ackBuf, (int)TransSize.ACKSIZE);
        }

        /// <summary>
        /// Send the specified buffer and size.
        /// </summary>
        /// <param name='buffer'>
        /// Buffer.
        /// </param>
        /// <param name='size'>
        /// Size.
        /// </param>
        public void send(byte[] buf, int size)
        {
            // TO DO Your own code
            //1. out-of-order packet is received, receiver send ACK 
            //2. corrupted packet is received, receiver sends double ACK 
            //Difference between 2.1 to 2.2 is:
            // - Receiver must include sequence number of the packet being 
            //acknowledge by an ACK message

            //Main loop
            int sendBytesIndex = 0;
            errorCount = 0;
            //KAN VÆRE FEJL HER, HAR SKIFTET DET TIL 1004!
            int sendBytesAmount = buffer.Length;
            //Should not be this large 

            var sendByteList = new List<byte>(buffer.Length);

            while (sendBytesIndex <= buf.Length)
            {
                if (sendBytesIndex + buffer.Length > buf.Length)
                    sendBytesAmount = buf.Length - sendBytesIndex;

                sendByteList.Clear();

                //Checksum
                sendByteList.Add((byte)0);
                sendByteList.Add((byte)0);
                sendByteList.Add((byte)seqNo); //Sequence
                sendByteList.Add((byte)0); //Data

                //Collection to insert 
                for (int i = 0; i < sendBytesAmount; i++)
                {
                    sendByteList.Add(buf[i]);
                }
                var sendByteArray = sendByteList.ToArray();

                Checksum.calcChecksum(ref sendByteArray, sendByteArray.Length);
                var slet2 = Encoding.ASCII.GetString(sendByteArray);

                //HER KAN SKE EN FEJL!
                link.send(sendByteArray, sendByteArray.Length);

                //Wait to receive ACK 
				old_seqNo = seqNo;
                bool ackReceived = receiveAck();
                if (ackReceived)
                {                    
                    //GIVER DET MENING?
                    sendBytesIndex += BUFSIZE + (int)TransSize.ACKSIZE;
                    errorCount = 0;
                }
                else
                {
                    Console.WriteLine("Error with ACK, probably wrong sequence");
                    if (errorCount < 5)
                    {
                        Console.WriteLine("Error count has exceeded 5 iterations!");
                        Console.WriteLine("Exiting...");
                        Environment.Exit(1);
                    }
                    errorCount++;
                }

            }
        }

        /// <summary>
        /// Receive the specified buffer.
        /// </summary>
        /// <param name='buffer'>
        /// Buffer.
        /// </param>
        public int receive(ref byte[] buf)
        {
            int bytesRead = 0;
            int preReadBytes = 0;
            var receiveBuffer = new byte[BUFSIZE + (int)TransSize.ACKSIZE];
            
            //Mangler offset for hvert read 
            while ((bytesRead += link.receive(ref receiveBuffer)) <= buf.Length)
            {
                if (bytesRead == -1)
                    sendAck((seqNo == (byte)0 ? false : true));
                
                seqNo = receiveBuffer[(byte)TransCHKSUM.SEQNO];

				if (Checksum.checkChecksum(receiveBuffer, receiveBuffer.Length))
				{
					sendAck((seqNo == (byte)0 ? true : false));
					var receivedBytes = new List<byte>(receiveBuffer);
					for (int i = 0; i < receivedBytes.Count - 4; i++)
					{
						buf[i + preReadBytes] = receiveBuffer[i + 4];
					}
					preReadBytes = bytesRead;
				}
                else
                {
                    sendAck((seqNo == (byte)0 ? false : true));
                }

				Array.Clear(receiveBuffer, 0, receiveBuffer.Length); 
            }
            return bytesRead;
        }
    }
}