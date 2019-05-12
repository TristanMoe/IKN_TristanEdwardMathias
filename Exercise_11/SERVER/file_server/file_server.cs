using System;
using System.IO;
using System.Text;
using Transportlaget;
using Linklaget;
using System.Threading; 
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
		private byte[] fileNameBuffer;

		private const string uglystring = "Woody equal ask saw sir weeks aware decay. Entrance prospect removing we packages strictly is no smallest he. For hopes may chief get hours day rooms. Oh no turned behind polite piqued enough at. Forbade few through inquiry blushes you. Cousin no itself eldest it in dinner latter missed no. Boisterous estimating interested collecting get conviction friendship say boy. Him mrs shy article smiling respect opinion excited. Welcomed humoured rejoiced peculiar to in an. Demesne far hearted suppose venture excited see had has. Dependent on so extremely delivered by. Yet ﻿no jokes worse her why. Bed one supposing breakfast day fulfilled off depending questions. Whatever boy her exertion his extended. Ecstatic followed handsome drawings entirely mrs one yet outweigh. Of acceptance insipidity remarkably is invitation. Ever man are put down his very. And marry may table him avoid. Hard sell it were into it upon. He forbade affixed parties of assured to me windows. Happiness him nor she disposing provision. Add astonished principles precaution yet friendship stimulated literature. State thing might stand one his plate. Offending or extremity therefore so difficult he on provision. Tended depart turned not are. Increasing impression interested expression he my at. Respect invited request charmed me warrant to. Expect no pretty as do though so genius afraid cousin. Girl when of ye snug poor draw. Mistake totally of in chiefly. Justice visitor him entered for. Continue delicate as unlocked entirely mr relation diverted in. Known not end fully being style house. An whom down kept lain name so at easy. Effects present letters inquiry no an removed or friends. Desire behind latter me though in. Supposing shameless am he engrossed up additions. My possible peculiar together to. Desire so better am cannot he up before points. Remember mistaken opinions it pleasure of debating. Court front maids forty if aware their at. Chicken use are pressed removed. Whether article spirits new her covered hastily sitting her. Money witty books nor son add. Chicken age had evening believe but proceed pretend mrs. At missed advice my it no sister. Miss told ham dull knew see she spot near can. Spirit her entire her called. Am of mr friendly by strongly peculiar juvenile. Unpleasant it sufficient simplicity am by friendship no inhabiting. Goodness doubtful material has denoting suitable she two. Dear mean she way and poor bred they come. He otherwise me incommode explained so in remaining. Polite barton in it warmly do county length an. Give lady of they such they sure it. Me contained explained my education. Vulgar as hearts by garret. Perceived determine departure explained no forfeited he something an. Contrasted dissimilar get joy you instrument out reasonably. Again keeps at no meant stuff. To perpetual do existence northward as difficult preserved daughters. Continued at up to zealously necessary breakfast. Surrounded sir motionless she end literature. Gay direction neglected but supported yet her. Was drawing natural fat respect husband. An as noisy an offer drawn blush place. These tried for way joy wrote witty. In mr began music weeks after at begin. Education no dejection so direction pretended household do to. Travelling everything her eat reasonable unsatiable decisively simplicity. Morning request be lasting it fortune demands highest of. Oh to talking improve produce in limited offices fifteen an. Wicket branch to answer do we. Place are decay men hours tiled. If or of ye throwing friendly required. Marianne interest in exertion as. Offering my branched confined oh dashwood. ";
       

		/// <summary>
		/// Initializes a new instance of the <see cref="file_server"/> class.
		/// </summary>
		private file_server ()
		{
			// TO DO Your own code
			var tpl = new Transport(BUFSIZE, APP);
			string filenameRequest;
			fileNameBuffer = new byte[BUFSIZE]; 

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
            long sizeOfFile = 0; 
			if((sizeOfFile = LIB.check_File_Exists(fileName)) == 0)
			{
				var fileDidNotExistBytes = Encoding.ASCII.GetBytes("");
				transport.send(fileDidNotExistBytes, fileDidNotExistBytes.Length);
				return; 
			}

			FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            
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
			try
			{
				//For application: 
				//new file_server();  

				#region Transport Layer Test
				//For Transport Layer test
                
					var t_server = new Transport(BUFSIZE, APP);
					var bytesToSend = new byte[BUFSIZE*10];
				    bytesToSend = Encoding.ASCII.GetBytes(uglystring + uglystring);
					t_server.send(bytesToSend, bytesToSend.Length);
				    Console.WriteLine(Encoding.ASCII.GetString(bytesToSend)); 
					Thread.Sleep(1000);
                
				
                #endregion

                #region Link Layer Test
                /*
                //For link layer test
				var link_server = new Link(BUFSIZE, APP);
				Thread.Sleep(1000); 
				string textToSend = "Alle Mennesker Skal Blive Til Aber!";
				while (true)
				{
					byte[] bytesToSend = Encoding.ASCII.GetBytes(uglystring);
					link_server.send(bytesToSend, bytesToSend.Length);
					Thread.Sleep(50);
					link_server.receive(ref bytesToSend);
					Console.WriteLine(Encoding.ASCII.GetString(bytesToSend));
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