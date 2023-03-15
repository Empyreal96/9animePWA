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
using Windows.Storage.Pickers;
using Windows.UI;
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
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        public MainPage()
        {
            this.InitializeComponent();
            ProgBar.Visibility = Visibility.Visible;
            var view = ApplicationView.GetForCurrentView();
            view.TryEnterFullScreenMode();

            InitMainViewEvents();
           // InitSettingsPageEvents();
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

            if (NavBar.Visibility == Visibility.Visible)
            {
                NavBar.Visibility = Visibility.Collapsed;
                RelativePanel.SetAlignTopWithPanel(MainView, true);


            }
            else
            {
                NavBar.Visibility = Visibility.Visible;

                RelativePanel.SetAlignTopWithPanel(MainView, false);
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
            if (BookmarksList.Items.Count != 0)
            {
                BookmarksList.Items.Clear();
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
        string Title;
        string PageUrl;
        private void BookmarksList_ItemClick(object sender, ItemClickEventArgs e)
        {
            selectedIndex = BookmarksList.SelectedIndex;
            Title = (e.ClickedItem as PageInfo).WebpageTitle;
            PageUrl = (e.ClickedItem as PageInfo).WebpageUrl;

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


        private async void BookmarkItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            BookmarksList.SelectedItem = senderElement.DataContext;

            selectedIndex = BookmarksList.SelectedIndex;

            var test = BookmarksList.SelectedItem as PageInfo;


            MessageDialog dialog = new MessageDialog($"{test.WebpageTitle}\n\nRemove this bookmark?");
            dialog.Commands.Add(new UICommand("Yes", null));
            dialog.Commands.Add(new UICommand("No", null));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            var cmd = await dialog.ShowAsync();

            if (cmd.Label == "Yes")
            {

                DeleteBookmark_Click();
            }
        }



        private async void DeleteBookmark_Click()
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
                await CustErr.ShowAsync();
            }
        }


        private async void DeleteBookmark_Click(object sender, RoutedEventArgs e)
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
                await CustErr.ShowAsync();
            }
        }



        bool IsHoldingPressed = false;
        private async void BookmarkItem_Holding(object sender, HoldingRoutedEventArgs e)
        {
            FrameworkElement senderElement = sender as FrameworkElement;
            BookmarksList.SelectedItem = senderElement.DataContext;

            selectedIndex = BookmarksList.SelectedIndex;

            var test = BookmarksList.SelectedItem as PageInfo;


            MessageDialog dialog = new MessageDialog($"{test.WebpageTitle}\n\nRemove this bookmark?");
            dialog.Commands.Add(new UICommand("Yes", null));
            dialog.Commands.Add(new UICommand("No", null));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            var cmd = await dialog.ShowAsync();

            if (cmd.Label == "Yes")
            {

                DeleteBookmark_Click();
            }
        }

        private void BookmarkFlyout_Closing(FlyoutBase sender, FlyoutBaseClosingEventArgs args)
        {
            /*if (IsHoldingPressed == true)
            {
                args.Cancel = true;
            }*/
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainView.CanGoBack)
            {
                MainView.GoBack();
            }
        }

        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainView.CanGoForward)
            {
                MainView.GoForward();
            }
        }

        private void RefreshPage_Click(object sender, RoutedEventArgs e)
        {
            MainView.Refresh();
        }


        private async void SaveColourSetting(string accentType) // Default or System
        {
                localSettings.Values["NavBarColour"] = accentType;
              
        }

        private async void LoadColourSetting()
        {
            var savedColour = localSettings.Values["NavBarColour"] as string;

            if (savedColour == "System")
            {
                var uiSettings = new Windows.UI.ViewManagement.UISettings();
                Windows.UI.Color accentColor = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);

                NavBar.Background = new SolidColorBrush(accentColor);
            } else
            {
                NavBar.Background = new SolidColorBrush(Colors.Gray);

                
            }
        }

        private void InitSettingsPageEvents()
        {

            LoadColourSetting();

            SettingsNavBarColour.Checked += SettingsNavBarColour_Checked;


        }

        private void SettingsNavBarColour_Checked(object sender, RoutedEventArgs e)
        {
            var uiSettings = new Windows.UI.ViewManagement.UISettings();
            Windows.UI.Color accentColor = uiSettings.GetColorValue(Windows.UI.ViewManagement.UIColorType.Accent);

           


            if (SettingsNavBarColour.IsChecked == true)
            {
                SaveColourSetting("System");
                NavBar.Background = new SolidColorBrush(accentColor);
            } else
            {
                SaveColourSetting("Default");
                NavBar.Background = new SolidColorBrush(Colors.Gray);
            }
        }

        private async void ExportBookmarks_Click(object sender, RoutedEventArgs e)
        {
            FolderPicker picker = new FolderPicker();
            picker.FileTypeFilter.Add(".bin");

            var ExportFolder = await picker.PickSingleFolderAsync();
            if (ExportFolder != null)
            {
                var bookmarkFile = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Bookmarks.bin");
                if (bookmarkFile != null)
                {
                    try
                    {
                        StorageFile Bookmarks = bookmarkFile as StorageFile;
                        await Bookmarks.CopyAsync(ExportFolder, "9anime_Bookmarks.bin", NameCollisionOption.GenerateUniqueName);
                    }
                    catch (Exception ex)
                    {
                        var CustErr = new MessageDialog($"{ex.Message}\n{ex.StackTrace}\n{ex.Source}");
                        CustErr.Commands.Add(new UICommand("Close"));
                        await CustErr.ShowAsync();
                    }
                    var CustErr2 = new MessageDialog($"Exported to \"{ExportFolder.Path}\" successfully");
                    CustErr2.Commands.Add(new UICommand("Close"));
                    await CustErr2.ShowAsync();

                }
                else
                {
                    var CustErr = new MessageDialog($"No saved bookmarks found");
                    CustErr.Commands.Add(new UICommand("Close"));
                    await CustErr.ShowAsync();
                }
            }
        }

        private async void ImportBookmarks_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".bin");

            var selectedFile = await picker.PickSingleFileAsync();
            if (selectedFile != null)
            {
                try
                {
                    await selectedFile.CopyAsync(ApplicationData.Current.LocalFolder, "Bookmarks.bin", NameCollisionOption.ReplaceExisting);
                }
                catch (Exception ex)
                {
                    var CustErr = new MessageDialog($"Failed importing bookmark\n\n{ex.Message}");
                    CustErr.Commands.Add(new UICommand("Close"));
                    await CustErr.ShowAsync();
                }
                ReadSavedBookmarks();
                var CustErr2 = new MessageDialog($"Imported {selectedFile.DisplayName} successfully");
                CustErr2.Commands.Add(new UICommand("Close"));
                await CustErr2.ShowAsync();
            }
        }

        private async void RemoveAllBookmarks_Click(object sender, RoutedEventArgs e)
        {
            MessageDialog dialog = new MessageDialog($"Remove all saved bookmarks?");
            dialog.Commands.Add(new UICommand("Yes", null));
            dialog.Commands.Add(new UICommand("No", null));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            var cmd = await dialog.ShowAsync();

            if (cmd.Label == "Yes")
            {
                var savedBookmarks = await ApplicationData.Current.LocalFolder.TryGetItemAsync("Bookmarks.bin");
                if (savedBookmarks != null)
                {
                    await savedBookmarks.DeleteAsync();


                    var CustErr2 = new MessageDialog($"Removed all bookmarks successfully");
                    CustErr2.Commands.Add(new UICommand("Close"));
                    await CustErr2.ShowAsync();
                }
            }
        }

        private void SettingsPage_Click(object sender, RoutedEventArgs e)
        {
            SettingsGrid.Visibility = Visibility.Visible;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            SettingsGrid.Visibility = Visibility.Collapsed;
        }
    }
}
