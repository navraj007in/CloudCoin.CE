using System;
using Xamarin.Forms;
using CloudCoin.CE.Droid;

[assembly: Dependency(typeof(FilePicker))]

namespace CloudCoin.CE.Droid
{
    public class FilePicker:IFilePicker
    {
        public FilePicker()
        {
        }

        public void pickFile()
        {
     
        }
    }
}
