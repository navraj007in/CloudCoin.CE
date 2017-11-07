using Android.App;
using Android.Content;
using Android.Runtime;
using Java.IO;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
using CloudCoin.CE;
using System.Runtime.CompilerServices;
using CloudCoin.CE.Droid;
using Uri = Android.Net.Uri;
using System.Linq;
using Xamarin.Forms;




[assembly: Xamarin.Forms.Dependency(typeof(FilePickerImplementation))]

namespace CloudCoin.CE.Droid
{
    /// <summary>
    /// Implementation for Feature
    /// </summary>
    ///
    [Preserve(AllMembers = true)]
    public class FilePickerImplementation : IFilePicker
    {
        public string GetHomeFolder()
        {
            return null;
        }

        public Task<FileData> PickFile(string importFilePath)
        {
            return null;
        }
    }
}