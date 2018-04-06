using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private int count =0;
        private WallpaperData wallpaperData;
        private Dictionary<string, WallpaperControl> views = new Dictionary<string, WallpaperControl>();
        private BlockingCollection<Wallpaper> queue = new BlockingCollection<Wallpaper>(new ConcurrentQueue<Wallpaper>());
        private App app;
        public MainWindow(WallpaperData wallpaperData, App app) {
            this.app = app;
            this.wallpaperData = wallpaperData;
            InitializeComponent();
            foreach(Wallpaper wallpaper in wallpaperData.Wallpapers) {
                queue.Add(wallpaper);
            }
            wallpaperData.WallpaperAdded += wallpaperAdded;
            wallpaperData.WallpaperRemoved += wallpaperRemoved;
            wallpaperData.WallpaperUpdated += wallpaperUpdated;
            new Thread(() => {
                Wallpaper wallpaper = null;
                while((wallpaper = queue.Take()) != Wallpaper.EMPTY_WALL) {
                    if(!views.ContainsKey(wallpaper.Path)) {
                        Dispatcher.Invoke(() => {
                            count++;
                            Title = count.ToString();
                            WallpaperControl control = new WallpaperControl(wallpaper);
                            control.MouseEnter += control_MouseEnter;
                            control.MouseLeave += control_MouseLeave;
                            wrapPanel.Children.Insert(0, control);
                            views.Add(wallpaper.Path, control);
                            control.ImageClicked += Control_ImageClicked;
                        });
                    } else {
                        Dispatcher.Invoke(() => {
                            wrapPanel.Children.Remove(views[wallpaper.Path]);
                            wrapPanel.Children.Insert(0, views[wallpaper.Path]);
                        });
                    }
                }
            }).Start();
            updateThumbButton();
        }


        void control_MouseLeave(object sender, MouseEventArgs e) {
            if(sender is WallpaperControl) {
                WallpaperControl control = sender as WallpaperControl;
                control.hideOptions();
            }
        }

        void control_MouseEnter(object sender, MouseEventArgs e) {
            if(sender is WallpaperControl) {
                WallpaperControl control = sender as WallpaperControl;
                control.showOptions();
            }            
        }

        private void wallpaperUpdated(object sender, Wallpaper wallpaper) {
            if(views.ContainsKey(wallpaper.Path)) {
                WallpaperControl wallpaperView = views[wallpaper.Path];
                wallpaperView.UpdateWallpaper();
            }
        }

        private void wallpaperRemoved(object sender, Wallpaper wallpaper) {
            if(views.ContainsKey(wallpaper.Path)) {
                Dispatcher.Invoke(() => {
                    wrapPanel.Children.Remove(views[wallpaper.Path]);
                    views.Remove(wallpaper.Path);
                });
            }
        }

        private void wallpaperAdded(object sender, Wallpaper wallpaper) {
            queue.Add(wallpaper);
        }

        private void closing(object sender, System.ComponentModel.CancelEventArgs e) {
            queue.Add(Wallpaper.EMPTY_WALL);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            optionPanel.Visibility = ((optionPanel.Visibility == System.Windows.Visibility.Collapsed) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e) {
            wallpaperData.disableWPF();
            app.Shutdown();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e) {
            wallpaperData.purge();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            wallpaperData.DeleteThumb = !wallpaperData.DeleteThumb;
            updateThumbButton();
        }

        private void updateThumbButton() {
            if(wallpaperData.DeleteThumb) {
                ThumbButton.Content = "Unused Thumbnails are Deleted";
            } else {
                ThumbButton.Content = "Unused Thumbnails are not Deleted";
            }
        }

        private WallpaperControl _displayedImage = null;

        private WallpaperControl DisplayedImage
        {
            get{ return _displayedImage; }
            set
            {
                if (value != null) {
                    Wallpaper wallpaper = value.Wallpaper;
                    BitmapImage img = new BitmapImage();
                    img.BeginInit();
                    img.UriSource = new Uri(wallpaper.Path);
                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.EndInit();
                    image.Source = img;
                    lblImageInfo.Content = wallpaper.Dimensions +" "+ wallpaper.Filesize;
                    lblImagePath.Content = wallpaper.Path;
                    refreshVisibility(wallpaper);

                }
                else
                {
                    image.Source = null;
                }
                _displayedImage = value;
            }
        }

        private void refreshVisibility(Wallpaper wallpaper)
        {
            btnFav.Visibility = !wallpaper.Favorited && wallpaper.Exists ? Visibility.Visible : Visibility.Hidden;
            lblFav.Visibility = wallpaper.Favorited ? Visibility.Visible : Visibility.Hidden;

            lblDeleted.Visibility= !wallpaper.Exists? Visibility.Visible : Visibility.Hidden;
            btnDelete.Visibility= wallpaper.Exists ? Visibility.Visible : Visibility.Hidden;
            lblConfirmDelete.Visibility = Visibility.Hidden;
            btnNo.Visibility = Visibility.Hidden;
            btnYes.Visibility = Visibility.Hidden;

            btnOpen.Visibility= wallpaper.Exists ? Visibility.Visible : Visibility.Hidden;
            btnFolder.Visibility = wallpaper.Exists ? Visibility.Visible : Visibility.Hidden;
        }

        private void Control_ImageClicked(object sender, RoutedEventArgs e)
        {
            if (sender is WallpaperControl)
            {
                WallpaperControl control = sender as WallpaperControl;
                // Winforms Image we want to get the WPF Image from...
                control.hideOptions();
                Panel.SetZIndex(popup, 3);
                DisplayedImage = control;
                e.Handled = true;
            }
        }

        private void popup_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Panel.SetZIndex(popup, 1);
            e.Handled = true;
            DisplayedImage = null;
        }

        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int delta = e.Delta > 0 ? -1 : 1;
            ShowPic(delta);
        }

        private void ShowPic(int delta)
        {
            if (DisplayedImage != null)
            {
                int index = wrapPanel.Children.IndexOf(DisplayedImage);
                WallpaperControl c = null;
                bool looped = false;
                do
                {
                    index += delta;

                    if (index < 0)
                    {
                        if (looped) { return; }
                        index = wrapPanel.Children.Count - 1;
                        looped = true;
                    }
                    if (index >= wrapPanel.Children.Count)
                    {
                        if (looped) { return; }
                        index = 0;
                        looped = true;
                    }
                    WallpaperControl c1 = (WallpaperControl)wrapPanel.Children[index];
                    if (c1.Wallpaper.Exists)
                    {
                        c = c1;
                    }
                } while (c == null);
                DisplayedImage = c;
            }
        }


        private void nextButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowPic(1);
            e.Handled = true;
        }
        private void prevButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ShowPic(-1);
            e.Handled = true;
        }

        private void btnFav_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayedImage != null)
            {
                DisplayedImage.btnFav.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                refreshVisibility(DisplayedImage.Wallpaper);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayedImage != null && DisplayedImage.Wallpaper.Exists)
            {
                lblConfirmDelete.Visibility = Visibility.Visible;
                btnDelete.Visibility = Visibility.Hidden;
                btnYes.Visibility = Visibility.Visible;
                btnNo.Visibility = Visibility.Visible;
            }

        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayedImage != null)
            {
                refreshVisibility(DisplayedImage.Wallpaper);
            }
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayedImage != null)
            {
                DisplayedImage.btnYes.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                refreshVisibility(DisplayedImage.Wallpaper);
            }

        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayedImage != null)
            {
                DisplayedImage.btnOpen.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                refreshVisibility(DisplayedImage.Wallpaper);
            }
        }

        private void btnFolder_Click(object sender, RoutedEventArgs e)
        {
            if (DisplayedImage != null)
            {
                DisplayedImage.btnFolder.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                refreshVisibility(DisplayedImage.Wallpaper);
            }

        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (DisplayedImage != null)
            {
                switch (e.Key)
                {
                    case Key.Left:
                        ShowPic(-1);
                        break;
                    case Key.Right:
                        ShowPic(1);
                        break;
                    case Key.Delete:
                        if (btnDelete.Visibility == Visibility.Visible) { 
                        btnDelete.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }else if(btnYes.Visibility == Visibility.Visible)
                        {
                            btnYes.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        }
                        break;
                    case Key.Enter:
                        btnOpen.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent));
                        break;
                    default:
                        break;
                }
            }
        }

    }
}
