using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.IO;
using Founders;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CloudCoin.CE
{
    public partial class CloudCoinPage : ContentPage
    {
        public static int timeout = 10000;
        FileUtils fileUtils = FileUtils.GetInstance("CloudCoin");
        string homefolder;
        public CloudCoinPage()
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            homefolder = documents + "/CloudCoin";
            try
            {
                Directory.CreateDirectory(homefolder);
            }
            catch(Exception e){
                Console.WriteLine(e.Message);
            }

            if (Device.RuntimePlatform == Device.iOS)
                fileUtils = FileUtils.GetInstance(homefolder);
            else
                fileUtils = FileUtils.GetInstance(homefolder);
            InitializeComponent();
            listFiles();

            fileUtils.CreateDirectoryStructure();
            Title = "CloudCoin CE ver 1.0";
            Task.Run(() => {
                echoRaida();
            });

            //var directories = Directory.EnumerateDirectories("./");
            //Directory.CreateDirectory("./CloudCoin");
            //var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            Console.WriteLine(("Folder Path- "+documents));
            Directory.CreateDirectory(documents + "/Bank");
            Directory.CreateDirectory(documents + "/Import");
            Directory.CreateDirectory(documents + "/Counterfeit");
            Directory.CreateDirectory(documents + "/Export");
            Directory.CreateDirectory(documents + "/Templates");
            Directory.CreateDirectory(documents + "/Suspect");
            Directory.CreateDirectory(documents + "/Trash");
            Directory.CreateDirectory(documents + "/Partial");
            Directory.CreateDirectory(documents + "/Imported");
            Directory.CreateDirectory(documents + "/Fracked");

            var directories = Directory.EnumerateDirectories(homefolder );

            //Directory.CreateDirectory("./cBank");

            foreach (var directory in directories)
            {
                Console.WriteLine(directory);
            }
        }

        public void listFiles() {
            var files = Directory.GetFiles("CloudCoin/Import");
            foreach (var file in files)
            {
                
                Console.WriteLine( file);
                if (File.Exists(fileUtils.importFolder  + Path.GetFileName(file)))
                    File.Delete(fileUtils.importFolder  + Path.GetFileName(file));
                
                File.Copy(file, fileUtils.importFolder + Path.GetFileName(file));
                Console.WriteLine("File Copied succesfully to " + fileUtils.importFolder + Path.GetFileName(file));
           
            }

            var files2 = Directory.GetFiles(fileUtils.suspectFolder);
            foreach (var file in files2)
            {
                Console.WriteLine("Target-"+file);
                File.Delete(file);

            }

            var filesc = Directory.GetFiles(fileUtils.counterfeitFolder);
            foreach (var file in filesc)
            {

                Console.WriteLine("Counterfeit-"+file);
            }
            var filesim = Directory.GetFiles(fileUtils.importedFolder);
            foreach (var file in filesim)
            {

                Console.WriteLine("Imported-" + file);
            }



        }
        public void multi_detect()
        {
            Console.Out.WriteLine("");
            Console.Out.WriteLine("  Detecting Authentication of Suspect Coins");// "Detecting Authentication of Suspect Coins");
            MultiDetect multi_detector = new MultiDetect(fileUtils);
            //multi_detector.txtLogs = txtLogs;

            //Calculate timeout
            int detectTime = 20000;
            if (RAIDA_Status.getLowest21() > detectTime)
            {
                detectTime = RAIDA_Status.getLowest21() + 200;
            }//Slow connection

            multi_detector.detectMulti(detectTime);
            // grade();
            // showCoins();

        }//end multi detect

        public void grade()
        {
            Console.Out.WriteLine("");
            updateLog("  Grading Authenticated Coins");
            Console.Out.WriteLine("  Grading Authenticated Coins");// "Detecting Authentication of Suspect Coins");
            Grader grader = new Grader(fileUtils);
            int[] detectionResults = grader.gradeAll(5000, 2000);
            //updateLog("  Total imported to bank: " + detectionResults[0]);
            //updateLog("  Total imported to fracked: " + detectionResults[1]);
            //updateLog("  Total Counterfeit: " + detectionResults[2]);
            //updateLog("  Total moved to Lost folder: " + detectionResults[4]);

            //Console.Out.WriteLine("  Total imported to bank: " + detectionResults[0]);//"Total imported to bank: "
            //Console.Out.WriteLine("  Total imported to fracked: " + detectionResults[1]);//"Total imported to fracked: "                                                                       // And the bank and the fractured for total
            //Console.Out.WriteLine("  Total Counterfeit: " + detectionResults[2]);//"Total Counterfeit: "
            //Console.Out.WriteLine("  Total Kept in suspect folder: " + detectionResults[3]);//"Total Kept in suspect folder: " 
            //Console.Out.WriteLine("  Total moved to Lost folder: " + detectionResults[4]);//"Total Kept in suspect folder: " 

        }//end detect


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
                multi_detect();
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
                //App.Current.Dispatcher.Invoke(delegate
                //{
                    
                    //cmdRestore.IsEnabled = true;
                    //cmdImport.IsEnabled = true;
                //});

            }
            else
            {
                DateTime before = DateTime.Now;
                DateTime after;
                TimeSpan ts = new TimeSpan();
                //Console.Out.WriteLine("  IMPORT DONE> NOW DETECTING MULTI. Do you want to start detecting?");// "No coins in import folder.");
                // Console.In.ReadLine();
                multi_detect();
                // Console.Out.WriteLine("  DETCATION DONE> NOW GRADING. Do you want to start Grading?");// "No coins in import folder.");
                // Console.In.ReadLine();
                after = DateTime.Now;
                ts = after.Subtract(before);//end the timer

                //grade();
                // Console.Out.WriteLine("  GRADING DONE NOW SHOWING. Do you wnat to show");// "No coins in import folder.");
                // Console.In.ReadLine();
                Console.Out.WriteLine("Time in ms to multi detect pown " + ts.TotalMilliseconds);
                RAIDA_Status.showMultiMs();
                showCoins();
        
            }//end if coins to import
        }   // end import

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            DirectoryInfo di = new DirectoryInfo(fileUtils.rootFolder);
            int count = di.GetDirectories().Length;
            var fileInfo = di.GetDirectories();
            foreach (var fi in fileInfo)
            {
                Console.WriteLine(fi.FullName);
            }
            Console.WriteLine("Length " + count);
            Task.Run(() => {
                import();

            });


            //DependencyService.Get<IFilePicker>().pickFile();
        }

        int[] bankTotals;
        int[] frackedTotals;
        int[] partialTotals;

        public void showCoins()
        {

            Console.Out.WriteLine("");
            // This is for consol apps.
            Banker bank = new Banker(fileUtils);
            bankTotals = bank.countCoins(fileUtils.counterfeitFolder);
            frackedTotals = bank.countCoins(fileUtils.frackedFolder);
            partialTotals = bank.countCoins(fileUtils.partialFolder);


            // int[] counterfeitTotals = bank.countCoins( counterfeitFolder );

            //setLabelText(lblOnesCount, Convert.ToString(bankTotals[1] + frackedTotals[1] + partialTotals[1]));
            //setLabelText(lblFivesCount, Convert.ToString(bankTotals[2] + frackedTotals[2] + partialTotals[2]));
            //setLabelText(lblQtrCount, Convert.ToString(bankTotals[3] + frackedTotals[3] + partialTotals[3]));
            //setLabelText(lblHundredCount, Convert.ToString(bankTotals[4] + frackedTotals[4] + partialTotals[4]));
            //setLabelText(lblTwoFiftiesCount, Convert.ToString(bankTotals[5] + frackedTotals[5] + partialTotals[5]));


            //setLabelText(lblOnesValue, Convert.ToString(bankTotals[1] + frackedTotals[1] + partialTotals[1]));
            //setLabelText(lblFivesValue, Convert.ToString((bankTotals[2] + frackedTotals[2] + partialTotals[2]) * 5));
            //setLabelText(lblQtrValue, Convert.ToString((bankTotals[3] + frackedTotals[3] + partialTotals[3]) * 25));
            //setLabelText(lblHundredValue, Convert.ToString((bankTotals[4] + frackedTotals[4] + partialTotals[4]) * 100));
            //setLabelText(lblTwoFiftiesValue, Convert.ToString((bankTotals[5] + frackedTotals[5] + partialTotals[5]) * 250));
            //setLabelText(lblTotalCoins, "Total Coins in Bank : " + Convert.ToString(bankTotals[0] + frackedTotals[0] + partialTotals[0]));

            /*setLabelText(lblNotesTotal, Convert.ToString(bankTotals[1] + frackedTotals[1] + partialTotals[1]
                + bankTotals[2] + frackedTotals[2] + partialTotals[2]
                + bankTotals[3] + frackedTotals[3] + partialTotals[3]
                + bankTotals[4] + frackedTotals[4] + partialTotals[4]
                + bankTotals[5] + frackedTotals[5] + partialTotals[5]));
*/
            //setLabelText(lblNotesTotal, Convert.ToString(bankTotals[0] + frackedTotals[0] + partialTotals[0]));
            //updateNotes();
        }// end show

        void Detector_OnUpdateStatus(object sender, ProgressEventArgs e)
        {
            updateLog(e.Status);
            Device.BeginInvokeOnMainThread(() => {
             //   lblProgressMessage.Text = e.Status + e.SecondStatus;
                if (e.SecondStatus != null)
                {
                    
                }
               //     lblProgressMessage2.Text = e.SecondStatus;
                //cmdImport.Text = e.SecondStatus;
               // ImportProgress.ProgressTo(e.percentage, 10000, Easing.Linear);
               /// lblProgress.Text = e.percentage + " % completed.";
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
            Console.WriteLine("100 % completed");

            Device.BeginInvokeOnMainThread(() => {
                Console.WriteLine("100 % completed");

                //ImportProgress.ProgressTo(100, 100, Easing.Linear);
                //lblProgress.Text = "100 % completed.";

            });
            //RefreshCoins?.Invoke(this, new EventArgs());


            //cmdRestore.IsEnabled = true;
            //  cmdImport.IsEnabled = true;
            //  progressBar.Value = 100;

        }//end detect
        private void updateLog(String msg)
        {
            Console.WriteLine("msg");
        }

        public void oldimport(int resume = 0)
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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            FrameExpoart.CornerRadius = (float)FrameExpoart.Height / 2;
            FrameImpoart.CornerRadius = (float)FrameImpoart.Height / 2;
            FrameSafe.CornerRadius = (float)FrameSafe.Height / 2;

            HelpEditor.Focused += (sender, e) => { HelpEditor.Unfocus(); };

        }

        void OnTappedSafe(object sender, EventArgs e)
        {
            FrameBackground.IsVisible = true;
            FrameSafeAction.IsVisible = true;
        }
        void OnTappedImport(object sender, EventArgs e)
        {
            int totalRAIDABad = 0;
            for (int i = 0; i < 25; i++)
            {
                if (RAIDA_Status.failsEcho[i])
                {
                    totalRAIDABad += 1;
                }
            }
            if (totalRAIDABad > 8)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Out.WriteLine("You do not have enough RAIDA to perform an import operation.");
                Console.Out.WriteLine("Check to make sure your internet is working.");
                Console.Out.WriteLine("Make sure no routers at your work are blocking access to the RAIDA.");
                Console.Out.WriteLine("Try to Echo RAIDA and see if the status has changed.");
                Console.ForegroundColor = ConsoleColor.White;

              
                return;
            }
            //cmdImport.IsEnabled = false;
            //cmdRestore.IsEnabled = false;
            //progressBar.Visibility = Visibility.Visible;

            Task.Run(() => { 
                import();
            }); 

            //FrameBackground.IsVisible = true;
            //FrameImportAction.IsVisible = true;
        }
        void OnTappedExport(object sender, EventArgs e)
        {
            FrameBackground.IsVisible = true;
            FrameExportAction.IsVisible = true;
        }
        async void OnTappedImportAction(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Message", "There are no coins to import. You have to get coins in form of .stack or abc.jpg/.jpeg files from email or AirDrop. Choose CloudCoin Scan&Safe icon to get files.", null, "OK");
            if (!answer)
            {
                FrameBackground.IsVisible = false;
                FrameImportAction.IsVisible = false;
            }

        }
        async void OnTappedExportAction(object sender, EventArgs e)
        {
            var answer = await DisplayAlert("Message", "Nothing to export.", null, "OK");
            if (!answer)
            {
                FrameBackground.IsVisible = false;
                FrameExportAction.IsVisible = false;
            }
        }

        void OnTappedCancel(object sender, EventArgs e)
        {
            FrameBackground.IsVisible = false;
            FrameImportAction.IsVisible = false;
            FrameSafeAction.IsVisible = false;
            FrameExportAction.IsVisible = false;
            FrameHelpAction.IsVisible = false;
        }
        void OnClickHelp(object sender, EventArgs e)
        {
            FrameBackground.IsVisible = true;
            FrameHelpAction.IsVisible = true;
        }

        void OnTappedJPGStack(object sender, EventArgs e)
        {
            if (sender as Label == ButtonJPG)
            {
                ButtonJPGFrame.BackgroundColor = Color.FromHex("#007AFF");
                ButtonJPG.TextColor = Color.White;
                ButtonStackFrame.BackgroundColor = Color.White;
                ButtonStackFrame.OutlineColor = Color.FromHex("#007AFF");
                ButtonStack.TextColor = Color.FromHex("#383F44");
            }
            else
            {
                ButtonStackFrame.BackgroundColor = Color.FromHex("#007AFF");
                ButtonStack.TextColor = Color.White;
                ButtonJPGFrame.BackgroundColor = Color.White;
                ButtonJPGFrame.OutlineColor = Color.FromHex("#007AFF");
                ButtonJPG.TextColor = Color.FromHex("#383F44");
            }
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
