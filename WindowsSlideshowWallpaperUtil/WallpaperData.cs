using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsSlideshowWallpaperUtil.Properties;

namespace WindowsSlideshowWallpaperUtil {
    public class WallpaperData {
        private const int MAX_WALLS = 100;
        private List<Wallpaper> wallpapers = new List<Wallpaper>();
        private string favDir;
        public EventHandler<Wallpaper> WallpaperAdded;
        public EventHandler<Wallpaper> WallpaperRemoved;
        public EventHandler<Wallpaper> WallpaperUpdated;
        private bool restart = false;
        public bool NeedsRestart { get { return restart; } }

        public string FavoriteDirectory { get { return favDir; } set { favDir = value; save(); } }

        public IEnumerable<Wallpaper> Wallpapers { get { return wallpapers; } }

        internal WallpaperData() {
            string savedFavDir = Properties.Settings.Default.FavDir;
            StringCollection walls = Properties.Settings.Default.Wallpapers;
            if(savedFavDir == null || savedFavDir == "") {
                savedFavDir = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures) + "\\favorite wallpapers";    
            }
            favDir = savedFavDir;
            if(walls != null && walls.Count > 0) {
                foreach(string wall in walls) {
                    wallpapers.Add(new Wallpaper(wall, this));
                }
            }
        }

        public void purge() {
            wallpapers.RemoveAll(wallpaper => { bool exist = wallpaper.Exists; if(!exist) { onRemove(wallpaper); } return !exist; });
        }

        private void onAdd(Wallpaper wallpaper) {
            if(WallpaperAdded != null) {
                WallpaperAdded(this, wallpaper);
            }
        }
        private void onRemove(Wallpaper wallpaper) {
            if(WallpaperRemoved != null) {
                wallpaper.deleteThumb();
                WallpaperRemoved(this, wallpaper);
            }
        }
        public void onUpdate(Wallpaper wallpaper) {
            if(WallpaperUpdated != null) {
                WallpaperUpdated(this, wallpaper);
            }
        }

        public void addWallpaper(string p) {
            try {
                if(File.Exists(p)) {
                    Wallpaper wallpaper = new Wallpaper(p, this);
                    wallpapers.Add(wallpaper);
                    while(wallpapers.Count > MAX_WALLS) {
                        onRemove(wallpapers[0]);
                        wallpapers.RemoveAt(0);
                    }
                    onAdd(wallpaper);
                    save();
                }
            } catch { }
        }

        private void save() {
            StringCollection walls = new StringCollection();
            foreach(Wallpaper wallpaper in wallpapers) {
                walls.Add(wallpaper.Path);
            }
            Properties.Settings.Default.Wallpapers = walls;
            Properties.Settings.Default.FavDir = FavoriteDirectory;
            Properties.Settings.Default.Save();
        }

        public void enableWPF() {
            Properties.Settings.Default.wpfEnabled = true;
            Properties.Settings.Default.Save();
            restart = true;
        }
        public void disableWPF() {
            Properties.Settings.Default.wpfEnabled = false;
            Properties.Settings.Default.Save();
            restart = true;
        }

        public bool WpfEnabled {
            get { return Properties.Settings.Default.wpfEnabled; }
        }

        public bool DeleteThumb{
            get { return Properties.Settings.Default.deleteThumbs; }
            set {
                Properties.Settings.Default.deleteThumbs = value; 
                Properties.Settings.Default.Save();
            }
        }

        public void resetRestart() {
            restart = false;
        }
    }
}
