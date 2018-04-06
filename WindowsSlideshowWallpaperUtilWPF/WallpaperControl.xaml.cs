using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WindowsSlideshowWallpaperUtil;

namespace WindowsSlideshowWallpaperUtilWPF {
    /// <summary>
    /// Interaction logic for WallpaperControl.xaml
    /// </summary>
    public partial class WallpaperControl : UserControl {
        private WindowsSlideshowWallpaperUtil.Wallpaper wallpaper;
        bool active, exist, favorited;

        public static readonly RoutedEvent imageClickedEvent = EventManager.RegisterRoutedEvent("imageClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WallpaperControl));

        public event RoutedEventHandler ImageClicked
        {
            add { AddHandler(imageClickedEvent, value); }
            remove { RemoveHandler(imageClickedEvent, value); }
        }

        public Wallpaper Wallpaper
        {
            get { return wallpaper; }
        }

        public WallpaperControl(WindowsSlideshowWallpaperUtil.Wallpaper wallpaper) {
            // TODO: Complete member initialization
            this.wallpaper = wallpaper;
            InitializeComponent();
            WallpaperImage.Source = Convert(wallpaper.Thumbnail);
            btnOpen.Content = wallpaper.Path;
            lblDimensions.Text = wallpaper.Dimensions;
            lblSize.Text = wallpaper.Filesize;
            check();
            hideOptions();
        }

        internal void UpdateWallpaper() {
            lblDimensions.Text = wallpaper.Dimensions;
            lblSize.Text = wallpaper.Filesize;
            bool active = this.active;
            hideOptions();
            check();
            if(active) { showOptions(); }
        }

        public void showOptions() {
            active = true;
            bool favorited = wallpaper.Favorited;
            btnFav.Visibility = (!favorited && btnFav.IsEnabled) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            btnDelete.Visibility = (btnDelete.IsEnabled) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            btnFolder.Visibility = (btnFolder.IsEnabled) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            btnOpen.Visibility = System.Windows.Visibility.Visible;
            btnYes.Visibility = System.Windows.Visibility.Hidden;
            btnNo.Visibility = System.Windows.Visibility.Hidden;
        }

        public void hideOptions() {
            active = false;
            lblConfirmDelete.Visibility = System.Windows.Visibility.Hidden;
            btnDelete.Visibility = System.Windows.Visibility.Hidden;
            btnFolder.Visibility = System.Windows.Visibility.Hidden;
            btnFav.Visibility = System.Windows.Visibility.Hidden;
            btnYes.Visibility = System.Windows.Visibility.Hidden;
            btnNo.Visibility = System.Windows.Visibility.Hidden;
            btnOpen.Visibility = System.Windows.Visibility.Hidden;
        }

        public BitmapImage Convert(System.Drawing.Image image) {
            // Winforms Image we want to get the WPF Image from...
            BitmapImage bitmap = new System.Windows.Media.Imaging.BitmapImage();
            bitmap.BeginInit();
            MemoryStream memoryStream = new MemoryStream();
            // Save to a memory stream...
            image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
            // Rewind the stream...
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
            bitmap.StreamSource = memoryStream;
            bitmap.EndInit();
            return bitmap;
        }

        private void WallpaperImage_MouseDown_1(object sender, MouseButtonEventArgs e) {
            if(e.ClickCount == 2 && e.LeftButton == MouseButtonState.Pressed) {
                if(wallpaper.Exists) {
                    RoutedEventArgs newEventArgs = new RoutedEventArgs(imageClickedEvent);
                    RaiseEvent(newEventArgs);
                    //System.Diagnostics.Process.Start(wallpaper.Path);
                }
            }
            e.Handled = true;
        }

        private void check() {
            exist = wallpaper.Exists;
            favorited = wallpaper.Favorited;
            lblFav.Visibility = (favorited) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Hidden;
            lblDeleted.Visibility = (exist) ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;
            btnFav.IsEnabled = exist && !favorited;
            btnDelete.IsEnabled = exist;
            btnFolder.IsEnabled = exist;
            btnYes.IsEnabled = exist;
            btnNo.IsEnabled = exist;
        }

        private void btnFav_Click(object sender, RoutedEventArgs e) {
            wallpaper.favorite();
            check();
        }


        private void btnFolder_Click(object sender, RoutedEventArgs e) {
            if(wallpaper.Exists) {
                ShowSelectedInExplorer.FileOrFolder(wallpaper.Path);
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (wallpaper.Exists)
            {
                System.Diagnostics.Process.Start(wallpaper.Path);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            btnNo.Visibility = System.Windows.Visibility.Visible;
            btnYes.Visibility = System.Windows.Visibility.Visible;
            btnDelete.Visibility = System.Windows.Visibility.Hidden;
            lblConfirmDelete.Visibility = System.Windows.Visibility.Visible;
        }

        private void btnNo_Click(object sender, RoutedEventArgs e) {
            btnNo.Visibility = System.Windows.Visibility.Hidden;
            btnYes.Visibility = System.Windows.Visibility.Hidden;
            btnDelete.Visibility = System.Windows.Visibility.Visible;
            lblConfirmDelete.Visibility = System.Windows.Visibility.Hidden;
        }

        private void btnYes_Click(object sender, RoutedEventArgs e) {
            btnNo.Visibility = System.Windows.Visibility.Hidden;
            btnYes.Visibility = System.Windows.Visibility.Hidden;
            btnDelete.Visibility = System.Windows.Visibility.Visible;
            lblConfirmDelete.Visibility = System.Windows.Visibility.Hidden;
            wallpaper.delete();
            check();

        }
    }
}
