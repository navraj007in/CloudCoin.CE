using Xamarin.Forms;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace CloudCoin.CE
{
    public partial class CloudCoinPage : ContentPage
    {
        public CloudCoinPage()
        {
            InitializeComponent();



            //FrameExpoart.CornerRadius = FrameExpoart.;
        }
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
            FrameBackground.IsVisible = true;
            FrameImportAction.IsVisible = true;
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
}
