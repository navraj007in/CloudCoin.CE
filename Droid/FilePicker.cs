using System;
using Xamarin.Forms;
using CloudCoin.CE.Droid;
using System.Threading.Tasks;

[assembly: Dependency(typeof(FilePicker))]

namespace CloudCoin.CE.Droid
{
    public class FilePicker:IFilePicker
    {
        public FilePicker()
        {
        }

        public string GetHomeFolder()
        {
            return "";  
        }

        public void pickFile()
        {
     
        }

        public Task<FileData> PickFile(string importFilePath)
        {
            throw new NotImplementedException();
        }
    }
}
