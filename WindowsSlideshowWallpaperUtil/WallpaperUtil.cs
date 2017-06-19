using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsSlideshowWallpaperUtil {
    public class WallpaperUtil {
        private readonly IWallpaperUtilSettings wallpaperUtilSettings;
        private readonly WallpaperData data;
        private List<string> lastWallpapers = new List<string>();

        public WallpaperData Data { get { return data; } }

        public WallpaperUtil(IWallpaperUtilSettings wallpaperUtilSettings) {
            this.data = new WallpaperData();
            this.wallpaperUtilSettings = wallpaperUtilSettings;
            int index = 0;
            foreach(Wallpaper wallpaper in data.Wallpapers) {
                lastWallpapers.Add(wallpaper.Path);
                index++;
                if(index == 10) {
                    break;
                }
            }
        }

        public void check() {
            try {
                List<string> currentWallpapers = new List<string>();
                currentWallpapers.AddRange(wallpaperUtilSettings.getCurrentWallpapers());
                foreach(string wallpaper in currentWallpapers) {
                    if(!lastWallpapers.Contains(wallpaper)) {
                        data.addWallpaper(wallpaper);
                    }
                }
                lastWallpapers = currentWallpapers;
            } catch {
            }
        }
    }
}
