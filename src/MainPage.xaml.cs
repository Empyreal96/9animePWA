using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace _9anime
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var view = ApplicationView.GetForCurrentView();
            view.TryEnterFullScreenMode();
            MainView.FrameNavigationStarting += MainView_FrameNavigationStarting;
            MainView.NewWindowRequested += MainView_NewWindowRequested;

            bool isNetworkConnected = NetworkInterface.GetIsNetworkAvailable();
            if (isNetworkConnected == true)
            {
            }
            else
            {
                var CustErr = new MessageDialog("Please connect to the internet");
                CustErr.Commands.Add(new UICommand("Close"));
                CustErr.ShowAsync();
            }

        }

        private void MainView_NewWindowRequested(WebView sender, WebViewNewWindowRequestedEventArgs args)
        {
            args.Handled = true;
        }

        private void MainView_FrameNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            try
            {
                Debug.WriteLine(args.Uri.ToString());
             if (!IsAllowedUri(args.Uri))
                  args.Cancel = true;
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private bool IsAllowedUri(Uri url)
        {
            if (url.ToString().Contains("vidstream.pro") ||
                url.ToString().Contains("filemoon.sx") ||
                url.ToString().Contains("streamtape.com") ||
                url.ToString().Contains("mp4upload.com") ||
                url.ToString().Contains("mcloud.to") ||
                url.ToString().Contains("9anime.to") ||
                url.ToString().Contains("9anime.me") ||
                url.ToString().Contains("9anime.pl") ||
                url.ToString().Contains("9anime.id") ||
                url.ToString().Contains("9anime.gs"))
            {
                return true;
            } else
            {
                return false;
            }
        }
    }
}
