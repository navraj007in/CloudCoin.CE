using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.IO;
using Founders;
using System.Linq;

namespace CloudCoin.CE.Views
{
    public partial class HomePage : ContentPage
    {
        FileUtils fileUtils = FileUtils.GetInstance("CloudCoin");   
        public HomePage()
        {
            InitializeComponent();
            fileUtils.CreateDirectoryStructure();
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
			import();

			//DependencyService.Get<IFilePicker>().pickFile();
		}

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
				//detect();
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
				//detect();
			}
        
//end if coins to import
		}   // end import


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
}
