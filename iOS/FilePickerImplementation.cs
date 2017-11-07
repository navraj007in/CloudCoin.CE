using Foundation;
using System;
using MobileCoreServices;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UIKit;
using System.Diagnostics;
using Xamarin.Forms;
using CloudCoin.CE.iOS;
using System.Net;
using Xamarin.Forms.PlatformConfiguration;

[assembly: Dependency(typeof(FilePickerImplementation))]
namespace CloudCoin.CE.iOS
{
    public class FilePickerImplementation : NSObject, IUIDocumentMenuDelegate, IFilePicker
    {
        private int _requestId;
        private TaskCompletionSource<FileData> _completionSource;

        /// <summary>
        /// Event which is invoked when a file was picked
        /// </summary>
        public EventHandler<FilePickerEventArgs> Handler
        {
            get;
            set;
        }

        private void OnFilePicked(FilePickerEventArgs e)
        {
            Handler?.Invoke(null, e);
        }

        public void DidPickDocumentPicker(UIDocumentMenuViewController documentMenu, UIDocumentPickerViewController documentPicker)
        {
            documentPicker.DidPickDocumentAtUrls += DocumentPicker_DidPickDocument;
            documentPicker.WasCancelled += DocumentPicker_WasCancelled;

            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(documentPicker, true, null);
        }

        private void DocumentPicker_DidPickDocument(object sender, UIDocumentPickedAtUrlsEventArgs e)
        {
            var securityEnabled = e.Urls[0].StartAccessingSecurityScopedResource();
            var doc = new UIDocument(e.Urls[0]);
            var data = NSData.FromUrl(e.Urls[0]);
            var dataBytes = new byte[data.Length];

            System.Runtime.InteropServices.Marshal.Copy(data.Bytes, dataBytes, 0, Convert.ToInt32(data.Length));

            string filename = doc.LocalizedName;
            string pathname = doc.FileUrl?.ToString();

            // iCloud drive can return null for LocalizedName.
            if (filename == null)
            {
                // Retrieve actual filename by taking the last entry after / in FileURL.
                // e.g. /path/to/file.ext -> file.ext

                // filesplit is either:
                // 0 (pathname is null, or last / is at position 0)
                // -1 (no / in pathname)
                // positive int (last occurence of / in string)
                var filesplit = pathname?.LastIndexOf('/') ?? 0;

                filename = pathname?.Substring(filesplit + 1);
            }



            var strContent = "";
            var webRequest = WebRequest.Create(pathname);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                strContent = reader.ReadToEnd();
            }


           // File.Copy(pathname, FilePickerImplementation.fileName + Path.DirectorySeparatorChar + filename);
            NSFileManager fileManager = NSFileManager.DefaultManager;
            OpenFile(doc.FileUrl);

            string path;
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            path = documents + "/CloudCoin/Import/";

            string createdfile = Path.Combine(path, filename);
            File.WriteAllText(createdfile, strContent);

            /*using (var streamWriter = new StreamWriter(createdfile))
            {
                streamWriter.WriteLine("");
            }*/





            //File.Copy(pathname,createdfile);

            //MoveFile(pathname, path + filename);

            OnFilePicked(new FilePickerEventArgs(dataBytes, filename, pathname));
        }

        /// <summary>
        /// Handles when the file picker was cancelled. Either in the
        /// popup menu or later on.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DocumentPicker_WasCancelled(object sender, EventArgs e)
        {
            {
                var tcs = Interlocked.Exchange(ref _completionSource, null);
                tcs.SetResult(null);
            }
        }







        #region Private Variables
        private nfloat _documentTextHeight = 0;
        #endregion

        #region Computed Properties
        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        /// <summary>
        /// Returns the delegate of the current running application
        /// </summary>
        /// <value>The this app.</value>
        public AppDelegate ThisApp
        {
            get { return (AppDelegate)UIApplication.SharedApplication.Delegate; }
        }
        #endregion


        #region Private Methods
        /// <summary>
        /// Moves a file from a given source url location to a given destination url.
        /// </summary>
        /// <returns><c>true</c>, if file was moved, <c>false</c> otherwise.</returns>
        /// <param name="fromURL">From UR.</param>
        /// <param name="toURL">To UR.</param>
        private bool MoveFile(string fromURL, string toURL)
        {
            bool successful = true;

            // Get source options
            var srcURL = NSUrl.FromFilename(fromURL);
            var srcIntent = NSFileAccessIntent.CreateReadingIntent(srcURL, NSFileCoordinatorReadingOptions.ForUploading);

            // Get destination options
            var dstURL = NSUrl.FromFilename(toURL);
            var dstIntent = NSFileAccessIntent.CreateReadingIntent(dstURL, NSFileCoordinatorReadingOptions.ForUploading);

            // Create an array
            var intents = new NSFileAccessIntent[] {
                srcIntent,
                dstIntent
            };

            // Initialize a file coordination with intents
            var queue = new NSOperationQueue();
            var fileCoordinator = new NSFileCoordinator();
            fileCoordinator.CoordinateAccess(intents, queue, (err) => {
                // Was there an error?
                if (err != null)
                {
                    // Yes, inform caller
                    Console.WriteLine("Error: {0}", err.LocalizedDescription);
                    successful = false;
                }
            });

            // Return successful
            return successful;
        }

        #endregion

        #region Private Methods
        /// <summary>
        ///  Adjust the size of the <c>DocumentText</c> text editor to account for the keyboard being displayed
        /// </summary>
        /// <param name="height">The new text area height</param>
        private void MoveDocumentText(nfloat height)
        {

            // Animate size change
            UIView.BeginAnimations("keyboard");
            UIView.SetAnimationDuration(0.3f);

            // Adjust frame to move the text away from the keyboard
           // DocumentText.Frame = new CGRect(0, DocumentText.Frame.Y, DocumentText.Frame.Width, height);

            // Start animation
            UIView.CommitAnimations();
        }
        #endregion


        public static string fileName = "";

        /// <summary>
        /// Lets the user pick a file with the systems default file picker
        /// For iOS iCloud drive needs to be configured
        /// </summary>
        /// <returns></returns>
        public async Task<FileData> PickFile(string importPath)
        {
            var media = await TakeMediaAsync();

            fileName = importPath;


            //var allowedUTIs = new string[] {

            //        UTType.UTF8PlainText,
            //        UTType.PlainText,
            //        UTType.RTF,
            //        UTType.PNG,
            //        UTType.Text,
            //        UTType.PDF,
            //        UTType.Image

            //    };

            //// Display the picker
            ////var picker = new UIDocumentPickerViewController (allowedUTIs, UIDocumentPickerMode.Open);
            //var pickerMenu = new UIDocumentMenuViewController(allowedUTIs, UIDocumentPickerMode.Open);
            //pickerMenu.DidPickDocumentPicker += (sender, args) => {

            //    // Wireup Document Picker
            //    args.DocumentPicker.DidPickDocument += (sndr, pArgs) => {

            //        // IMPORTANT! You must lock the security scope before you can
            //        // access this file
            //        var securityEnabled = pArgs.Url.StartAccessingSecurityScopedResource();

            //        // Open the document
            //        ThisApp.OpenDocument(pArgs.Url);

            //        // TODO: This should work but doesn't
            //        // Apple's WWDC 2014 sample project does this but it blows
            //        // up in Xamarin
            //        NSFileCoordinator fileCoordinator = new NSFileCoordinator();
            //        NSError err;
            //        fileCoordinator.CoordinateRead(pArgs.Url, 0, out err, (NSUrl newUrl) => {
            //            NSData data = NSData.FromUrl(newUrl);
            //            Console.WriteLine("Data: {0}", data);
            //        });

            //        // IMPORTANT! You must release the security lock established
            //        // above.
            //        pArgs.Url.StopAccessingSecurityScopedResource();
            //    };

            //    // Display the document picker
            //    UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(args.DocumentPicker, true, null);
            //};

            //pickerMenu.ModalPresentationStyle = UIModalPresentationStyle.Popover;
            //UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(pickerMenu, true, null);
            ////UIPopoverPresentationController presentationPopover = pickerMenu.PopoverPresentationController;
            ////if (presentationPopover != null)
            ////{
            ////    presentationPopover.SourceView = this.View;
            ////    presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Down;

            ////    // Get UIBarButtonItem's frame
            ////    // there is no built in way to get a UIBarButtonItem frame so you need to hack your own
            ////    // This is because UIBarButtonItem inherits from UIBarItem and UIBarItem inherits from NSObject
            ////    var buttonView = (UIView)((UIBarButtonItem)s).ValueForKey(new NSString("view"));
            ////    presentationPopover.SourceRect = buttonView.Frame;
            ////}

            return media;
        }

        private Task<FileData> TakeMediaAsync()
        {
            var id = GetRequestId();

            var ntcs = new TaskCompletionSource<FileData>(id);

            if (Interlocked.CompareExchange(ref _completionSource, ntcs, null) != null)
                throw new InvalidOperationException("Only one operation can be active at a time");

            var allowedUtis = new string[] {
                UTType.UTF8PlainText,
                UTType.PlainText,
                UTType.RTF,
                UTType.PNG,
                UTType.Text,
                UTType.PDF,
                UTType.Image,
                UTType.UTF16PlainText,
                UTType.FileURL,
                UTType.JSON
            };

            var importMenu =
                new UIDocumentMenuViewController(allowedUtis, UIDocumentPickerMode.Import)
                {
                    Delegate = this,
                    ModalPresentationStyle = UIModalPresentationStyle.Popover
                };

            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(importMenu, true, null);

            var presPopover = importMenu.PopoverPresentationController;

            if (presPopover != null)
            {
                presPopover.SourceView = UIApplication.SharedApplication.KeyWindow.RootViewController.View;
                presPopover.PermittedArrowDirections = UIPopoverArrowDirection.Down;
            }

            Handler = null;

            Handler = (s, e) => {
                var tcs = Interlocked.Exchange(ref _completionSource, null);

                tcs?.SetResult(new FileData(e.FilePath, e.FileName, () => File.OpenRead(e.FilePath)));
            };

            return _completionSource.Task;
        }

        public void WasCancelled(UIDocumentMenuViewController documentMenu)
        {
            var tcs = Interlocked.Exchange(ref _completionSource, null);

            tcs?.SetResult(null);
        }

        private int GetRequestId()
        {
            var id = _requestId;

            if (_requestId == int.MaxValue)
                _requestId = 0;
            else
                _requestId++;

            return id;
        }

        public async Task<bool> SaveFile(FileData fileToSave)
        {
            try
            {
                var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                var fileName = Path.Combine(documents, fileToSave.FileName);

                File.WriteAllBytes(fileName, fileToSave.DataArray);

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        public void OpenFile(NSUrl fileUrl)
        {
            var docControl = UIDocumentInteractionController.FromUrl(fileUrl);

            var window = UIApplication.SharedApplication.KeyWindow;
            var subViews = window.Subviews;
            var lastView = subViews.Last();
            var frame = lastView.Frame;

            docControl.PresentOpenInMenu(frame, lastView, true);
        }

        public void OpenFile(string fileToOpen)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var fileName = Path.Combine(documents, fileToOpen);

            if (NSFileManager.DefaultManager.FileExists(fileName))
            {
                var url = new NSUrl(fileName, true);
                OpenFile(url);
            }
        }

        public async void OpenFile(FileData fileToOpen)
        {
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var fileName = Path.Combine(documents, fileToOpen.FileName);

            if (NSFileManager.DefaultManager.FileExists(fileName))
            {
                var url = new NSUrl(fileName, true);

                OpenFile(url);
            }
            else
            {
                await SaveFile(fileToOpen);
                OpenFile(fileToOpen);
            }
        }

        public string GetHomeFolder()
        {
            return Android.OS.Environment.ExternalStorageDirectory.ToString() + "/CloudCoin";
        }
    }
}