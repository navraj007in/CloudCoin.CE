using System;
using System.Collections.Generic;

using Xamarin.Forms;
using System.IO;
using Founders;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using CloudCoin.CE.Interface;

namespace CloudCoin.CE
{
    public partial class CloudCoinPage : ContentPage
    {
        void Handle_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            updateExportTotal();
        }

        public void updateExportTotal() {
            int oneCount = onePicker.SelectedIndex ;
            int fiveCount = (fivePicker.SelectedIndex )*5;
            int qtrCount = (qtrPicker.SelectedIndex )*25;
            int hundredCount = (hundredPicker.SelectedIndex )*100;
            int twoFiftyCount = (twoFiftyPicker.SelectedIndex )*250;
            lblExportValue.Text = "Export " + (oneCount + fiveCount + qtrCount + hundredCount+ twoFiftyCount);
        }
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

            fileUtils.CreateDirectoryStructure();

            if (Device.RuntimePlatform == Device.iOS)
                listFiles();//commented by srajan 

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
            Directory.CreateDirectory(documents + "/Sent");

            var directories = Directory.EnumerateDirectories(homefolder );

            //Directory.CreateDirectory("./cBank");

            foreach (var directory in directories)
            {
                Console.WriteLine(directory);
            }
            showCoins();
            updateExportTotal();
        }
        public void fillPickers(String oneCount, String fiveCount, String qtrCount, String hundredCount, String twoFiftyCount){
            for (int i = 0; i <= Convert.ToInt16(oneCount);i++) {
                onePicker.Items.Add(Convert.ToString(i));
            }

            for (int i = 0; i <= Convert.ToInt16(fiveCount); i++)
            {
                fivePicker.Items.Add(Convert.ToString(i));
            }

            for (int i = 0; i <= Convert.ToInt16(qtrCount); i++)
            {
                qtrPicker.Items.Add(Convert.ToString(i));
            }

            for (int i = 0; i <= Convert.ToInt16(hundredCount); i++)
            {
                hundredPicker.Items.Add(Convert.ToString(i));
            }


            for (int i = 0; i <= Convert.ToInt16(twoFiftyCount); i++)
            {
                twoFiftyPicker.Items.Add(Convert.ToString(i));
            }
            onePicker.SelectedIndex = 0;
            fivePicker.SelectedIndex = 0;
            qtrPicker.SelectedIndex = 0;
            hundredPicker.SelectedIndex = 0;
            twoFiftyPicker.SelectedIndex = 0;

        }
        public void listFiles() {
            var files = Directory.GetFiles(fileUtils.importFolder);
            foreach (var file in files)
            {
                
                Console.WriteLine( file);
                //if (File.Exists(fileUtils.importFolder  + Path.GetFileName(file)))
                  //  File.Delete(fileUtils.importFolder  + Path.GetFileName(file));
                
                //File.Copy(file, fileUtils.importFolder + Path.GetFileName(file));
                //Console.WriteLine("File Copied succesfully to " + fileUtils.importFolder + Path.GetFileName(file));
           
            }

            var tempfiles = Directory.GetFiles("CloudCoin/Templates");
            foreach (var file in tempfiles)
            {

                Console.WriteLine(file);
                if (File.Exists(fileUtils.templateFolder + Path.GetFileName(file)))
                    File.Delete(fileUtils.templateFolder + Path.GetFileName(file));

                File.Copy(file, fileUtils.templateFolder + Path.GetFileName(file));
                Console.WriteLine("File Copied succesfully to " + fileUtils.templateFolder + Path.GetFileName(file));

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

            var tfilesc = Directory.GetFiles(fileUtils.exportFolder);
            foreach (var file in tfilesc)
            {

                Console.WriteLine("Export-" + file);
            }

            var tfilesce = Directory.GetFiles(fileUtils.sentFolder);
            foreach (var file in tfilesce)
            {

                Console.WriteLine("Export-" + file);
            }

            var tfilescep = Directory.GetFiles(fileUtils.bankFolder);
            foreach (var file in tfilescep)
            {

                Console.WriteLine("Partial-" + file);
            }

            var trfilescep = Directory.GetFiles(fileUtils.trashFolder);
            foreach (var file in trfilescep)
            {

                Console.WriteLine("Trash-" + file);
            }

        }
        public void multi_detect()
        {
            Console.Out.WriteLine("");
            Console.Out.WriteLine("  Detecting Authentication of Suspect Coins");// "Detecting Authentication of Suspect Coins");
            MultiDetect multi_detector = new MultiDetect(fileUtils);
            multi_detector.importBar = importbar;
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

                grade();
                // Console.Out.WriteLine("  GRADING DONE NOW SHOWING. Do you wnat to show");// "No coins in import folder.");
                // Console.In.ReadLine();
                Console.Out.WriteLine("Time in ms to multi detect pown " + ts.TotalMilliseconds);
                RAIDA_Status.showMultiMs();
                showCoins();
        
            }

            FrameBackground.IsVisible = false;
                FrameImportAction.IsVisible = false;
   
            Device.BeginInvokeOnMainThread(() =>
            {
                //indicator.IsVisible = false;

            });
          
            //end if coins to import
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

            lblOneCount.Text = Convert.ToString(bankTotals[1] + frackedTotals[1] + partialTotals[1]);
            lblFiveCount.Text = Convert.ToString(bankTotals[2] + frackedTotals[2] + partialTotals[2]);
            lblQtrCount.Text = Convert.ToString(bankTotals[3] + frackedTotals[3] + partialTotals[3]);
            lblHundredCount.Text = Convert.ToString(bankTotals[4] + frackedTotals[4] + partialTotals[4]);
            lblTwoFiftyCount.Text = Convert.ToString(bankTotals[5] + frackedTotals[5] + partialTotals[5]);

            lblBankTotal.Text = Convert.ToString(bankTotals[0] + frackedTotals[0] + partialTotals[0]);
            lblExportTotal.Text = Convert.ToString(bankTotals[0] + frackedTotals[0] + partialTotals[0]);

            fillPickers(lblOneCount.Text, lblFiveCount.Text,lblQtrCount.Text,lblHundredCount.Text,lblTwoFiftyCount.Text);
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
        bool isJpeg = true;
        bool isStack = true;
        public int exportJpegStack = 1;

        public void export()
        {
            if (isJpeg == true)
                exportJpegStack = 1;
            else
                exportJpegStack = 2;

            Banker bank = new Banker(fileUtils);
            int[] bankTotals = bank.countCoins(fileUtils.bankFolder);
            int[] frackedTotals = bank.countCoins(fileUtils.frackedFolder);
            int[] partialTotals = bank.countCoins(fileUtils.partialFolder);

            //updateLog("  Your Bank Inventory:");
            int grandTotal = (bankTotals[0] + frackedTotals[0] + partialTotals[0]);
            // state how many 1, 5, 25, 100 and 250
            int exp_1 = onePicker.SelectedIndex;
            int exp_5 = fivePicker.SelectedIndex;
            int exp_25 = qtrPicker.SelectedIndex;
            int exp_100 = hundredPicker.SelectedIndex;
            int exp_250 = twoFiftyPicker.SelectedIndex;
            //Warn if too many coins

            if (exp_1 + exp_5 + exp_25 + exp_100 + exp_250 == 0)
            {
                Console.WriteLine("Can not export 0 coins");
                return;
            }

            //updateLog(Convert.ToString(bankTotals[1] + frackedTotals[1] + bankTotals[2] + frackedTotals[2] + bankTotals[3] + frackedTotals[3] + bankTotals[4] + frackedTotals[4] + bankTotals[5] + frackedTotals[5] + partialTotals[1] + partialTotals[2] + partialTotals[3] + partialTotals[4] + partialTotals[5]));

            if (((bankTotals[1] + frackedTotals[1]) + (bankTotals[2] + frackedTotals[2]) + (bankTotals[3] + frackedTotals[3]) + (bankTotals[4] + frackedTotals[4]) + (bankTotals[5] + frackedTotals[5]) + partialTotals[1] + partialTotals[2] + partialTotals[3] + partialTotals[4] + partialTotals[5]) > 1000)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Out.WriteLine("Warning: You have more than 1000 Notes in your bank. Stack files should not have more than 1000 Notes in them.");
                Console.Out.WriteLine("Do not export stack files with more than 1000 notes. .");
                //updateLog("Warning: You have more than 1000 Notes in your bank. Stack files should not have more than 1000 Notes in them.");
                //updateLog("Do not export stack files with more than 1000 notes. .");

                Console.ForegroundColor = ConsoleColor.White;
            }//end if they have more than 1000 coins

            Console.Out.WriteLine("  Do you want to export your CloudCoin to (1)jpgs or (2) stack (JSON) file?");
            int file_type = 0; //reader.readInt(1, 2);





            Exporter exporter = new Exporter(fileUtils);
            //exporter.OnUpdateStatus += Exporter_OnUpdateStatus; ;
            file_type = exportJpegStack;

            String tag = txtTag.Text;// reader.readString();
            //Console.Out.WriteLine(("Exporting to:" + exportFolder));

            if (file_type == 1)
            {
                exporter.writeJPEGFiles(exp_1, exp_5, exp_25, exp_100, exp_250, tag);
                // stringToFile( json, "test.txt");
            }
            else
            {
                exporter.writeJSONFile(exp_1, exp_5, exp_25, exp_100, exp_250, tag);
            
            }

            emailExportFiles();

            // end if type jpge or stack

            //RefreshCoins?.Invoke(this, new EventArgs());
            //updateLog("Exporting CloudCoins Completed.");
            showCoins();
            //Process.Start(fileUtils.exportFolder);
            lblExportValue.Text = "Export 0";

            //MessageBox.Show("Export completed.", "Cloudcoins", MessageBoxButtons.OK);
        }// end export One

        private void emailExportFiles() {
            var tfilesc = Directory.GetFiles(fileUtils.exportFolder);
            DependencyService.Get<Mailer>().SendMail(fileUtils.exportFolder, tfilesc);
            foreach (var file in tfilesc)
            {
                File.Copy(file, fileUtils.sentFolder + Path.GetFileName(file));
                File.Delete(file);
                Console.Out.WriteLine(file);
            }


        }
        void OnTappedSafe(object sender, EventArgs e)
        {
            FrameBackground.IsVisible = true;
            FrameSafeAction.IsVisible = true;
        }
        async void OnTappedImport(object sender, EventArgs e)
        {


            try
            {
                var iFilePicker = DependencyService.Get<IFilePicker>();
                FileData filedata = await iFilePicker.PickFile(fileUtils.importFolder);

                //indicator.IsVisible = true;


                if (filedata !=null){
                    ActivityIndicator ai = new ActivityIndicator();
                    ai.IsRunning = true;
                    ai.IsEnabled = true;
                    ai.BindingContext = this;
                    ai.SetBinding(ActivityIndicator.IsVisibleProperty, "IsBusy");
                    import();
                    ai.IsRunning = false;
                    ai.IsEnabled = true;
                }
                    

              

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Debug.WriteLine(ex);
            }

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
        void OnTappedExportAction(object sender, EventArgs e)
        {
            export();
          /*  var answer = await DisplayAlert("Message", "Nothing to export.", null, "OK");
            if (!answer)
            {
                FrameBackground.IsVisible = false;
                FrameExportAction.IsVisible = false;
            }
            */
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
                isJpeg = true;
                isStack = false;
            }
            else
            {
                ButtonStackFrame.BackgroundColor = Color.FromHex("#007AFF");
                ButtonStack.TextColor = Color.White;
                ButtonJPGFrame.BackgroundColor = Color.White;
                ButtonJPGFrame.OutlineColor = Color.FromHex("#007AFF");
                ButtonJPG.TextColor = Color.FromHex("#383F44");
                isJpeg = false;
                isStack = true;
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
