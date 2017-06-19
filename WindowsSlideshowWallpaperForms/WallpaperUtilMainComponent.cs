using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsSlideshowWallpaperUtil;
using System.Threading;
using WindowsSlideshowWallpaperForms;

namespace WindowsSlideshowWallpaperUtilForms {
    public partial class WallpaperUtilMainComponent : Component {
        private readonly ApplicationContext appContext = new ApplicationContext();
        private WallpaperListForm form = null;
        private WallpaperUtil wallpaperUtil;
        bool first = true;

        public ApplicationContext AppContext { get { return appContext; } }

        public WallpaperUtilMainComponent(WallpaperUtil wallpaperUtil) {
            this.wallpaperUtil = wallpaperUtil;
            InitializeComponent();
            toolStripMenuItem1.Click += new EventHandler(toolStripMenuItem1_Click);
            toolStripMenuItem2.Click += new EventHandler(toolStripMenuItem2_Click);
            toolStripMenuItem3.Click += new EventHandler(toolStripMenuItem3_Click);
            notifyIcon1.DoubleClick += new EventHandler(notifyIcon1_Click);
            appContext.ThreadExit += new EventHandler(appContext_ThreadExit);
            timer1.Start();
            //showWindow();
        }

        void form_FormClosing(object sender, FormClosingEventArgs e) {
            notifyIcon1.ShowBalloonTip(4000, "Still running", "Wallpaper monitor is still running\n To open the window again double click this icon,\n For options right click this icon.", ToolTipIcon.Info);
        }

        void toolStripMenuItem3_Click(object sender, EventArgs e) {
            folderBrowserDialog1.SelectedPath = wallpaperUtil.Data.FavoriteDirectory;
            if(DialogResult.OK == folderBrowserDialog1.ShowDialog()) {
                wallpaperUtil.Data.FavoriteDirectory = folderBrowserDialog1.SelectedPath;
            }
        }

        void appContext_ThreadExit(object sender, EventArgs e) {
            WallpaperListForm form1 = this.form;
            if(form1 != null && (!form1.IsDisposed || !form1.Visible)) {
                try {
                    form1.Close();
                } catch(Exception) { }
            }
            notifyIcon1.Visible = false;
            timer1.Stop();
        }

        void notifyIcon1_Click(object sender, EventArgs e) {
            showWindow();
        }

        void toolStripMenuItem2_Click(object sender, EventArgs e) {
            appContext.ExitThread();
        }

        void toolStripMenuItem1_Click(object sender, EventArgs e) {
            showWindow();
        }

        private void timer1_Tick(object sender, System.EventArgs e) {
            wallpaperUtil.check();
        }

        private void showWindow() {
            WallpaperListForm form1 = this.form;
            if(form1 != null && (!form1.IsDisposed || !form1.Visible)) {
                try {
                    form1.Close();
                } catch(Exception) { }
            }
            form1 = new WallpaperListForm(wallpaperUtil.Data, this);
            form1.Visible = true;
            this.form = form1;
            if(first) {
                form.FormClosing += new FormClosingEventHandler(form_FormClosing);
                first = false;
            }
        }
    }
}
