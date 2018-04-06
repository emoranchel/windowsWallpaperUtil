using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using WindowsSlideshowWallpaperUtil;

namespace WindowsSlideshowWallpaperUtilWPF {
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    [Serializable]
    public class App : System.Windows.Application {

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private WallpaperUtil wallpaperUtil;
        private MainWindow mainWindow;
        private bool running = true;


        public App(WallpaperUtil util) {
            this.wallpaperUtil = util;
            Startup += Application_Startup_1;
            ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            Exit += Application_Exit_1;
            DispatcherUnhandledException += Application_DispatcherUnhandledException_1;
            new Thread(() => { while(running) { wallpaperUtil.check(); Thread.Sleep(300); } }).Start();
        }

        private void Application_Startup_1(object sender, StartupEventArgs e) {
            InitializeFormComponent();
            notifyIcon1.Visible = true;
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            toolStripMenuItem2.Click += toolStripMenuItem2_Click;
            toolStripMenuItem3.Click += toolStripMenuItem3_Click;
            notifyIcon1.DoubleClick += notifyIcon1_Click;
            showWindow();
        }

        void toolStripMenuItem3_Click(object sender, EventArgs e) {
            folderBrowserDialog1.SelectedPath = wallpaperUtil.Data.FavoriteDirectory;
            if(DialogResult.OK == folderBrowserDialog1.ShowDialog()) {
                wallpaperUtil.Data.FavoriteDirectory = folderBrowserDialog1.SelectedPath;
            }
        }

        void notifyIcon1_Click(object sender, EventArgs e) {
            showWindow();
        }

        void toolStripMenuItem2_Click(object sender, EventArgs e) {
            Shutdown();
        }

        void toolStripMenuItem1_Click(object sender, EventArgs e) {
            showWindow();
        }
        bool first = true;
        private void showWindow() {
            MainWindow form1 = this.mainWindow;
            if(form1 != null && (form1.IsLoaded || form1.IsVisible)) {
                try {
                    form1.Close();
                } catch(Exception) { }
            }
            form1 = new MainWindow(wallpaperUtil.Data, this);
            form1.Show();
            this.mainWindow = form1;
            if(first) {
                mainWindow.Closing += mainWindowClosing;
                first = false;
            }
        }

        private void mainWindowClosing(object sender, System.ComponentModel.CancelEventArgs e) {
            notifyIcon1.ShowBalloonTip(4000, "Still running", "Wallpaper monitor is still running\n To open the window again double click this icon,\n For options right click this icon.", ToolTipIcon.Info);
        }


        private void Application_Exit_1(object sender, ExitEventArgs e) {
            running = false;
            notifyIcon1.Visible = false;
        }

        private void Application_DispatcherUnhandledException_1(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e) {
            notifyIcon1.ShowBalloonTip(4000, "Error!", "Bad Joo Joo hapened:\n"+e.Exception.Message+"\nChill we got this!", ToolTipIcon.Info);
            e.Handled = true;
        }

        private void InitializeFormComponent() {
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.contextMenuStrip1.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.BalloonTipText = "Wallpaper monitor is running.";
            this.notifyIcon1.BalloonTipTitle = "WallpaperMonitor";
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = WindowsSlideshowWallpaperUtilWPF.Properties.Resources.computer_32;
            this.notifyIcon1.Text = "Recent Wallpapers";
            this.notifyIcon1.Icon = WindowsSlideshowWallpaperUtilWPF.Properties.Resources.Live_TV;
            this.notifyIcon1.Visible = true;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem3,
            this.toolStripMenuItem2});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(189, 70);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem1.Text = "Show window";
            this.toolStripMenuItem1.ToolTipText = "Opens the wallpaper options window.";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem3.Text = "Set favorites directory";
            this.toolStripMenuItem3.ToolTipText = "Sets the favorites directory to save favorite wallpapers.";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(188, 22);
            this.toolStripMenuItem2.Text = "Exit";
            this.toolStripMenuItem2.ToolTipText = "Close the wallpaper monitor.";
            this.contextMenuStrip1.ResumeLayout(false);
        }
    }
}
