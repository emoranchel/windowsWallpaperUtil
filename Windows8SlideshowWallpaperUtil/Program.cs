using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsSlideshowWallpaperUtil;
using WindowsSlideshowWallpaperUtilForms;
using WindowsSlideshowWallpaperUtilWPF;

namespace Windows8SlideshowWallpaperUtil {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread()]
        [LoaderOptimization(LoaderOptimization.MultiDomainHost)]
        static void Main() {
            WallpaperUtil wallpaperUtil = new WallpaperUtil(new Windows8WallpaperUtilSettings());
            if(wallpaperUtil.Data.WpfEnabled) {
                App appWpf = new App(wallpaperUtil);
                appWpf.Run();
            } else {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                WallpaperUtilMainComponent appWf = new WallpaperUtilMainComponent(wallpaperUtil);
                Application.Run(appWf.AppContext);
            }
            if(wallpaperUtil.Data.NeedsRestart) {
                Process.Start(Application.ExecutablePath);
            }
        }
    }
}
