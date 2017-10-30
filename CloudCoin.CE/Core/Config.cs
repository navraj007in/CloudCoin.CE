using System;
using System.IO;

namespace CloudCoinCore
{
    public class Config
    {
        public static string HomeFolder = "CloudCoin";
        public static String DefaultHomeLocation = "";
		public static String importFolder =  "Import" ;
		public static String importedFolder = "Imported" ;
		public static String trashFolder =  "Trash" ;
		public static String suspectFolder = "Suspect" ;
		public static String frackedFolder = "Fracked" ;
		public static String bankFolder = "Bank" ;
		public static String templateFolder = "Templates";
		public static String counterfeitFolder = "Counterfeit" ;
		public static String directoryFolder = "Directory" ;
		public static String exportFolder = "Export" ;
		public static String languageFolder = "Language" ;
		public static String partialFolder = "Partial" ;
		public static String detectedFolder = "Detected";
		public static String recieptsFolder = "Reciepts";
        public static String lostFolder = "Lost";
        public static String sentFolder = "Sent";



		public static String WorkSpaceKey = "workspace";
        public static String DisclaimerKey = "isDisclaimerShown";
        public Config()
        {
        }
    }
}
