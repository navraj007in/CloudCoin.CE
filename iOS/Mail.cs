using System;
using Xamarin.Forms;
using CloudCoin.CE.iOS;

using CloudCoin.CE.Interface;
using MessageUI;
using UIKit;
using System.Drawing;
using Foundation;

[assembly: Dependency(typeof(Mail))]

namespace CloudCoin.CE.iOS
{
    public class Mail : Mailer
    {
        MFMailComposeViewController mailController;

        public Mail()
        {
        }

        public string GetHomeFolder()
        {
            return Android.OS.Environment.ExternalStorageDirectory.ToString() + "/CloudCoin";
        }

        public void SendMail(string folder, string[] filenames) {

            var title = new UILabel(new RectangleF(-110, 80, 320, 30));
            title.Font = UIFont.SystemFontOfSize(24.0f);
            title.TextAlignment = UITextAlignment.Center;
            title.TextColor = UIColor.Black;
            title.Text = "Contact";

            if (MFMailComposeViewController.CanSendMail)
            {

                mailController = new MFMailComposeViewController();

                // do mail operations here
                mailController.SetToRecipients(new string[] { "" });
                mailController.SetSubject("Export CloudCoins");
                mailController.SetMessageBody("", false);
                foreach (var file in filenames)
                {
                    NSData data = NSData.FromFile(file);
                mailController.AddAttachmentData(data, "application/txt", System.IO.Path.GetFileName(file));
                
                }
                mailController.Finished += (object s, MFComposeResultEventArgs args) =>
                {
                    Console.WriteLine(args.Result.ToString());
                    args.Controller.DismissViewController(true, null);
                };

                UIApplication.SharedApplication.KeyWindow.RootViewController.PresentViewController(mailController, true, null);
            }
            else { 
                Console.WriteLine("Email can't be sent"); 
            }


        }
    }
}
