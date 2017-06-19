using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsSlideshowWallpaperUtil;
using WindowsSlideshowWallpaperUtilForms;
using WindowsSlideshowWallpaperUtilWPF;

namespace Windows7SlideshowWallpaperUtil {
    static class Program {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            WallpaperUtil wallpaperUtil = new WallpaperUtil(new Windows7WallpaperUtilSettings());
            if(wallpaperUtil.Data.WpfEnabled) {
                // Create new instance of application subclass
                App app = new App(wallpaperUtil);

                // Start running the application
                app.Run();

            } else {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                WallpaperUtilMainComponent MainComponent = new WallpaperUtilMainComponent(wallpaperUtil);
                Application.Run(MainComponent.AppContext);
            }
            if(wallpaperUtil.Data.NeedsRestart) {
                Process.Start(Application.ExecutablePath);
            }
        }
    }
}
