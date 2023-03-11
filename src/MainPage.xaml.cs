using DataManager;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
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

namespace _9Anime_PWA
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public class PageInfo
        {
            public string WebpageTitle { get; set; }
            public string WebpageUrl { get; set; }
        }

        public ObservableCollection<PageInfo> SavedBookmarks;
        public string CurrentUrl { get; set; }
        public string CurrentDocumentTitle { get; set; }
        bool IsFirstStart = true;
        ApplicationDataContainer localsettings = ApplicationData.Current.LocalSettings;
        public MainPage()
        {
            this.InitializeComponent();
            ProgBar.Visibility = Visibility.Visible;
            var view = ApplicationView.GetForCurrentView();
            view.TryEnterFullScreenMode();

            InitMainViewEvents();
            ReadSavedBookmarks();
            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;




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

            IsFirstStart = false;
        }

        private async void InitMainViewEvents()
        {

            MainView.FrameNavigationStarting += MainView_FrameNavigationStarting;
            MainView.NewWindowRequested += MainView_NewWindowRequested;

        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {

            if (MainView.CanGoBack)
            {
                MainView.GoBack();
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
            }
            else
            {
                return false;
            }
        }

        private void MainView_ContainsFullScreenElementChanged(WebView sender, object args)
        {

            if (BookmarkButton.Visibility == Visibility.Visible)
            {
                BookmarkButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                BookmarkButton.Visibility = Visibility.Visible;

            }
        }



        public async void ReadSavedBookmarks()
        {
            ObservableCollection<PageInfo> restoredBookmarks = await LocalDataManager.GetData<ObservableCollection<PageInfo>>("Bookmarks.bin");
            if (restoredBookmarks != null)
            {
                SavedBookmarks = restoredBookmarks;
            }
            else
            {
                SavedBookmarks = new ObservableCollection<PageInfo>();
               
            }

            foreach (var item in SavedBookmarks)
            {
                BookmarksList.Items.Add(item);
            }

           // BookmarksList.ItemsSource = SavedBookmarks;
        }

        public async void SaveBookmarks()
        {
            await LocalDataManager.SaveData("Bookmarks.bin", SavedBookmarks);
        }


        private void AddBookmarkBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var webpage = CurrentUrl;
                var documentTitle = CurrentDocumentTitle;

                if (BookmarksList.Items.Count != 0)
                {
                    BookmarksList.Items.Clear();
                }
                SavedBookmarks.Add(new PageInfo
                {
                    WebpageTitle = documentTitle,
                    WebpageUrl = webpage
                });

                
                foreach (var item in SavedBookmarks)
                {
                    BookmarksList.Items.Add(item);
                }


                //BookmarksList.ItemsSource = SavedBookmarks;

                SaveBookmarks();
            }
            catch (Exception ex)
            {
                var CustErr = new MessageDialog($"{ex.Message}\n{ex.StackTrace}\n{ex.Source}\n\n{CurrentUrl}\n\n{BookmarksList.Items.Count}");
                CustErr.Commands.Add(new UICommand("Close"));
                CustErr.ShowAsync();
            }

        }
        private void BookmarksList_ItemClick(object sender, ItemClickEventArgs e)
        {
            selectedIndex = BookmarksList.SelectedIndex;
            string Title = (e.ClickedItem as PageInfo).WebpageTitle;
            string PageUrl = (e.ClickedItem as PageInfo).WebpageUrl;

            MainView.Navigate(new Uri(PageUrl));
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
                {
                    args.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void MainView_NavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {

            if (IsFirstStart == true)
            {

            }
            else
            {
                ProgBar.Visibility = Visibility.Visible;
                BookmarkButton.IsEnabled = true;

            }

            Debug.WriteLine(args.Uri.ToString());
            string url = args.Uri.ToString();
            CurrentUrl = url;
            CurrentUrlBox.Text = url;



        }

        private void MainView_FrameNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            string title;
            if (MainView.DocumentTitle.Contains("Watch Anime"))
            {
                title = MainView.DocumentTitle.Replace("Watch Anime", "");
            }
            else
            {
                title = MainView.DocumentTitle;
            }

            CurrentDocumentTitle = title;
            DocumentTitleBox.Text = title;


            ProgBar.Visibility = Visibility.Collapsed;
            BookmarkButton.IsEnabled = true;


        }









        private void LoadingBarEnabled(bool value)
        {
            if (value == true)
            {
                ProgBar.Visibility = Visibility.Visible;
                ProgBar.IsIndeterminate = true;
                BookmarkButton.IsEnabled = false;
            }
            else
            {
                ProgBar.Visibility = Visibility.Collapsed;
                ProgBar.IsIndeterminate = false;
                BookmarkButton.IsEnabled = true;
            }
        }


        int selectedIndex;
        private void BookmarksList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            //selectedIndex = BookmarksList.SelectedIndex;
        }

        private void DeleteBookmark_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                BookmarksList.Items.RemoveAt(selectedIndex);

                SavedBookmarks.Clear();
                foreach (PageInfo item in BookmarksList.Items)
                {
                    SavedBookmarks.Add(item);
                }

                SaveBookmarks();
            }
            catch (Exception ex)
            {
                var CustErr = new MessageDialog($"{ex.Message}\n{ex.StackTrace}\n{ex.Source}\n\n{selectedIndex}");
                CustErr.Commands.Add(new UICommand("Close"));
                CustErr.ShowAsync();
            }
        }
        bool IsHoldingPressed = false;
        private void BookmarksList_Holding(object sender, HoldingRoutedEventArgs e)
        {
           // IsHoldingPressed = true;
            BookmarkRightClick.ShowAt(MainView, e.GetPosition(MainView));
            //IsHoldingPressed = false;
           // BookmarkFlyout.ShowAt(BookmarkButton);
        }

        private void BookmarkFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            /*if (IsHoldingPressed == true)
            {
                args.Cancel = true;
            }*/
        }
    }
}
