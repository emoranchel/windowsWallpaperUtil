using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsSlideshowWallpaperUtil;

namespace Windows8SlideshowWallpaperUtil {
    class WallpaperTestUtilSettings : IWallpaperUtilSettings {
        FileInfo[] files;
        int index = 0;
        public WallpaperTestUtilSettings() {
            DirectoryInfo dir = new DirectoryInfo("D:\\pictures\\konachan\\Questionable");
            files = dir.GetFiles();
        }

        public string[] getCurrentWallpapers() {
            if(index % 50 == 0) {
                Thread.Sleep(20000);
            }
            if(index >= files.Length) {
                index = 0;
            }
            string[] file = new string[]{files[index].FullName};
            index++;
            return file;
        }
    }
}
