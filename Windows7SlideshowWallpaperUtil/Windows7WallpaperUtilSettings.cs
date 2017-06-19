using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsSlideshowWallpaperUtil;

namespace Windows7SlideshowWallpaperUtil {
    class Windows7WallpaperUtilSettings : IWallpaperUtilSettings {
        public string[] getCurrentWallpapers() {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Internet Explorer\\Desktop\\General");
            return new String[]{key.GetValue("WallpaperSource").ToString()};
        }

    }
}
