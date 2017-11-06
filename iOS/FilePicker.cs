using System;
using Xamarin.Forms;
using CloudCoin.CE.iOS;
using System.IO;
using System.Threading.Tasks;
using UIKit;
using ELCImagePicker;
using System.Collections.Generic;

//[assembly: Dependency(typeof(FilePicker))]

namespace CloudCoin.CE.iOS
{
    public class FilePicker// : IFilePicker
    {
        public FilePicker()
        {
        }

        public void pickFile()
        {
            
			Console.WriteLine(Environment.SpecialFolder.Personal);

			var picker = ELCImagePickerViewController.Create(15);
            
			//picker.MaximumImagesCount = 15;

			picker.Completion.ContinueWith(t => {
				if (t.IsCanceled || t.Exception != null)
				{
					// no pictures for you!
				}
				else
				{
					var items = t.Result as List<AssetResult>;
				}
			});

			//var rootController = ((AppDelegate)(UIApplication.SharedApplication.Delegate)).
            //                                                                              Window.RootViewController.
            //                                                                              ChildViewControllers[0].
            //                                                                              ChildViewControllers[1].
             //                                                                             ChildViewControllers[0];

            UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(picker, true, null);
			//var navcontroller = rootController as UINavigationController;
			//if (navcontroller != null)
			//	rootController = navcontroller.VisibleViewController;

            //rootController.PresentViewController(picker, true, null);
            //return picker;
			//PresentViewController(picker, true, null);
	
        }

	
		private void DocVC_WasCancelled(object sender, EventArgs e)
		{
			//Handle being cancelled 
		}

		private void DocVC_DidPickDocument(object sender, UIDocumentPickedEventArgs e)
		{
			//Handle document selection
		}
    }
}
