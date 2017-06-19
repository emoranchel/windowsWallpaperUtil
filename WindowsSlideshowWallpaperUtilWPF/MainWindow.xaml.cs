using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    }
}
