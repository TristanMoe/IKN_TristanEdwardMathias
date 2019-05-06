using System;
using Linklaget;

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

		/// <summary>
		/// Initializes a new instance of the <see cref="Transport"/> class.
		/// </summary>
		public Transport (int BUFSIZE, string APP)
		{
			link = new Link(BUFSIZE+(int)TransSize.ACKSIZE, APP);
			Checksum = new Checksum();
			buffer = new byte[BUFSIZE+(int)TransSize.ACKSIZE];
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

			if (recvSize == (int)TransSize.ACKSIZE) {
				dataReceived = false;
				if (!Checksum.checkChecksum (buffer, (int)TransSize.ACKSIZE) ||
				  buffer [(int)TransCHKSUM.SEQNO] != seqNo ||
				  buffer [(int)TransCHKSUM.TYPE] != (int)TransType.ACK)
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
		private void sendAck (bool ackType)
		{
			byte[] ackBuf = new byte[(int)TransSize.ACKSIZE];
			ackBuf [(int)TransCHKSUM.SEQNO] = (byte)
				(ackType ? (byte)buffer [(int)TransCHKSUM.SEQNO] : (byte)(buffer [(int)TransCHKSUM.SEQNO] + 1) % 2);
			ackBuf [(int)TransCHKSUM.TYPE] = (byte)(int)TransType.ACK;
			Checksum.calcChecksum (ref ackBuf, (int)TransSize.ACKSIZE);
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
			int sendBytesAmount = 1000; 
			//Should not be this large 
			byte[] sendByteArray = new byte[2000];
			byte[] receiveByteArray = new byte[2000]; 

            while(sendBytesIndex < buf.Length)
			{
				Array.Clear(sendByteArray, 0, sendByteArray.Length); 
                
                if (sendBytesIndex+1000 > buf.Length)
					sendBytesAmount =  buf.Length - sendBytesIndex;
               
                //Array sourceArray, long sourceIndex, Array destinationArray, long destinationIndex, long length         
				Array.Copy(buf, sendBytesIndex, sendByteArray, 4, sendBytesAmount);
				//Insert Sequence 
				buf[(int)TransCHKSUM.SEQNO] = seqNo;  //Sequence

                          
				//Insert Type 
				buf[(int)TransCHKSUM.TYPE] = 0; //Data type 

				Checksum.calcChecksum(ref sendByteArray, sendByteArray.Length);
				link.send(sendByteArray, sendByteArray.Length);

               //Wait to receive ACK 
                bool ackReceived = receiveAck(); 
                if(ackReceived)
				{
					old_seqNo = seqNo;
					sendBytesIndex += 1000;
					seqNo = (seqNo == (byte)0 ? (byte)1 : (byte)0); 
					errorCount = 0; 
				}
				else
				{
					if (errorCount <= 5)
					{
						Console.WriteLine("Error count has exceeded 5 iterations!");
						Console.WriteLine("Exiting...");
						return; 
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
		public int receive (ref byte[] buf)
		{
			int bytesRead = 0;
			int preReadBytes = 0; 
                        
            //Mangler offset for hvert read 
			while((bytesRead += link.receive(ref buffer)) <= buf.Length)
			{
				seqNo = buffer[(byte)TransCHKSUM.SEQNO];

                if (Checksum.checkChecksum(buf, buf.Length))
                    sendAck(seqNo == (byte)0 ? false : true);
                else
                    sendAck(seqNo == (byte)0 ? true : false);

				Array.Copy(buffer, 0, buf, preReadBytes, buffer.Length);
				preReadBytes = bytesRead; 
				Array.Clear(buf, 0, buf.Length); 
                
			}
			return bytesRead;
        }
	}
}