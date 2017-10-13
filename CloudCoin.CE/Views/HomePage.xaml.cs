using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.IO;
using Founders;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CloudCoin.CE.Views
{
    public partial class HomePage : ContentPage
    {
        public static int timeout = 10000;
		FileUtils fileUtils = FileUtils.GetInstance("CloudCoin");   
        public HomePage()
        {
            InitializeComponent();
            fileUtils.CreateDirectoryStructure();
            Title = "CloudCoin CE ver 1.0";
			Task.Run(() => {
				echoRaida();
			});
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(fileUtils.rootFolder);
            int count = di.GetDirectories().Length;
            var fileInfo = di.GetDirectories();
            foreach (var fi in fileInfo) {
                Console.WriteLine(fi.FullName);
            }
            Console.WriteLine("Length " + count);
            Task.Run(() => {
				import();
                
			});
			
            
			//DependencyService.Get<IFilePicker>().pickFile();
		}

		void Detector_OnUpdateStatus(object sender, ProgressEventArgs e)
		{
			updateLog(e.Status);
			Device.BeginInvokeOnMainThread(() => {
                lblProgressMessage.Text = e.Status +  e.SecondStatus;
                if(e.SecondStatus!=null)
                    lblProgressMessage2.Text = e.SecondStatus;
                //cmdImport.Text = e.SecondStatus;
				ImportProgress.ProgressTo(e.percentage, 10000, Easing.Linear);
                lblProgress.Text = e.percentage + " % completed.";
   			});
        }

		public void detect()
		{
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			Console.Out.WriteLine("");
			updateLog("  Detecting Authentication of Suspect Coins");

			Console.Out.WriteLine("  Detecting Authentication of Suspect Coins");// "Detecting Authentication of Suspect Coins");
			Detector detector = new Detector(fileUtils, timeout);

			detector.OnUpdateStatus += Detector_OnUpdateStatus;

			int[] detectionResults = detector.detectAll();
			Console.Out.WriteLine("  Total imported to bank: " + detectionResults[0]);//"Total imported to bank: "
																					  //Console.Out.WriteLine("  Total imported to fracked: " + detectionResults[2]);//"Total imported to fracked: "
			updateLog("  Total imported to bank: " + detectionResults[0]);
			//updateLog("  Total imported to fracked: " + detectionResults[2]);
			// And the bank and the fractured for total
			Console.Out.WriteLine("  Total Counterfeit: " + detectionResults[1]);//"Total Counterfeit: "
			Console.Out.WriteLine("  Total Kept in suspect folder: " + detectionResults[3]);//"Total Kept in suspect folder: " 
			updateLog("  Total Counterfeit: " + detectionResults[1]);
			updateLog("  Total Kept in suspect folder: " + detectionResults[3]);
			updateLog("  Total Notes imported to Bank: " + detector.totalImported);

			//            showCoins();
			stopwatch.Stop();
			Console.Out.WriteLine(stopwatch.Elapsed + " ms");
			updateLog("Time to import " + detectionResults[0] + " Coins: " + stopwatch.Elapsed.ToCustomString() + "");

            Device.BeginInvokeOnMainThread(() => {

				ImportProgress.ProgressTo(100, 100, Easing.Linear);
                lblProgress.Text =  "100 % completed.";

			});
			//RefreshCoins?.Invoke(this, new EventArgs());

            
			//cmdRestore.IsEnabled = true;
			//  cmdImport.IsEnabled = true;
			//  progressBar.Value = 100;

		}//end detect
		private void updateLog(String msg){
            
        }

		public void import(int resume = 0)
		{

			//Check RAIDA Status

			//CHECK TO SEE IF THERE ARE UN DETECTED COINS IN THE SUSPECT FOLDER
			String[] suspectFileNames = new DirectoryInfo(fileUtils.suspectFolder).GetFiles().Select(o => o.Name).ToArray();//Get all files in suspect folder
			if (suspectFileNames.Length > 0)
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Out.WriteLine("  Finishing importing coins from last time...");//
				updateLog("  Finishing importing coins from last time...");

				Console.ForegroundColor = ConsoleColor.White;
				detect();
				Console.Out.WriteLine("  Now looking in import folder for new coins...");// "Now looking in import folder for new coins...");
				updateLog("  Now looking in import folder for new coins...");
			} //end if there are files in the suspect folder that need to be imported


			Console.Out.WriteLine("");
			Console.ForegroundColor = ConsoleColor.Green;
			Console.Out.WriteLine("  Loading all CloudCoins in your import folder: ");// "Loading all CloudCoins in your import folder: " );
			Console.Out.WriteLine(fileUtils.importFolder);
			updateLog("  Loading all CloudCoins in your import folder: ");
			updateLog(fileUtils.importFolder);

			Console.ForegroundColor = ConsoleColor.White;
			Importer importer = new Importer(fileUtils);
			if (!importer.importAll() && resume == 0)//Moves all CloudCoins from the Import folder into the Suspect folder. 
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Out.WriteLine("  No coins in import folder.");// "No coins in import folder.");
				updateLog("No coins in import Folder");

				Console.ForegroundColor = ConsoleColor.White;
	

			}
			else
			{
				detect();
			}
        
//end if coins to import
		}   // end import

		public bool echoRaida()
		{
			RAIDA_Status.resetEcho();
			RAIDA raida1 = new RAIDA();
			Response[] results = raida1.echoAll(15000);
			int totalReady = 0;
			Console.Out.WriteLine("");
			//For every RAIDA check its results
			int longestCountryName = 15;

			Console.Out.WriteLine();
			for (int i = 0; i < 25; i++)
			{
				int padding = longestCountryName - RAIDA.countries[i].Length;
				string strPad = "";
				for (int j = 0; j < padding; j++)
				{
					strPad += " ";
				}//end for padding
				 // Console.Out.Write(RAIDA_Status.failsEcho[i]);
				if (RAIDA_Status.failsEcho[i])
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.Out.Write(strPad + RAIDA.countries[i]);
				}
				else
				{
					Console.ForegroundColor = ConsoleColor.Green;
					Console.Out.Write(strPad + RAIDA.countries[i]);
					totalReady++;
				}
				if (i == 4 || i == 9 || i == 14 || i == 19) { Console.WriteLine(); }
			}//end for
			Console.ForegroundColor = ConsoleColor.White;
			Console.Out.WriteLine("");
			Console.Out.WriteLine("");
			Console.Out.Write("  RAIDA Health: " + totalReady + " / 25: ");//"RAIDA Health: " + totalReady );

			//Check if enough are good 
			if (totalReady < 16)//
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.Out.WriteLine("  Not enough RAIDA servers can be contacted to import new coins.");// );
				Console.Out.WriteLine("  Is your device connected to the Internet?");// );
				Console.Out.WriteLine("  Is a router blocking your connection?");// );
				Console.ForegroundColor = ConsoleColor.White;
				return false;
			}
			else
			{
				Console.ForegroundColor = ConsoleColor.Green;
				Console.Out.WriteLine("The RAIDA is ready for counterfeit detection.");// );
				Console.ForegroundColor = ConsoleColor.White;
				return true;
			}//end if enough RAIDA
		}//End echo

		void HandleExport_Clicked(object sender, System.EventArgs e)
        {
			DirectoryInfo di = new DirectoryInfo("Import");
			int count = di.GetFiles().Length;
			
            foreach (var fi in di.GetFiles())
			{
				Console.WriteLine(fi.FullName);
			}
			Console.WriteLine("Length " + count);
            //Console.WriteLine(fileInfo[0].FullName);
        }
    }
	public static class MyExtensions
	{
		public static string ToCustomString(this TimeSpan span)
		{
			return string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
		}
	}
	
}
