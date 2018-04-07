using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsSlideshowWallpaperUtil;

namespace WindowsSlideshowWallpaperUtilForms {
    internal partial class WallpaperListForm : Form {
        Dictionary<string, WallpaperView> views = new Dictionary<string, WallpaperView>();
        BlockingCollection<Wallpaper> queue = new BlockingCollection<Wallpaper>(new ConcurrentQueue<Wallpaper>());

        internal WallpaperListForm(WallpaperData data, WallpaperUtilMainComponent application) {
            this.application = application;
            this.data = data;
            InitializeComponent();
            foreach(Wallpaper wallpaper in data.Wallpapers) {
                setTitle(flowLayoutPanel1.Controls.Count + "(" + queue.Count + ")");
                queue.Add(wallpaper);
            }
            data.WallpaperAdded += wallpaperAdded;
            data.WallpaperRemoved += wallpaperRemoved;
            data.WallpaperUpdated += wallpaperUpdated;
            backgroundWorker1.RunWorkerAsync();
            updateThumbButton();
        }

        private void wallpaperUpdated(object sender, Wallpaper wallpaper) {
            if(views.ContainsKey(wallpaper.Path)) {
                WallpaperView wallpaperView = views[wallpaper.Path];
                wallpaperView.UpdateWallpaper();
            }
        }

        private void wallpaperRemoved(object sender, Wallpaper wallpaper) {
            if(views.ContainsKey(wallpaper.Path)) {
                flowLayoutPanel1.Controls.Remove(views[wallpaper.Path]);
                views.Remove(wallpaper.Path);
            }
        }

        private void wallpaperAdded(object sender, Wallpaper wallpaper) {
            queue.Add(wallpaper);
        }

        private object lastEntered = null;
        private WallpaperData data;
        private WallpaperUtilMainComponent application;
        void wallpaperView_MouseEnter(object sender, EventArgs e) {
            if(lastEntered != sender) {
                foreach(Control control in flowLayoutPanel1.Controls) {
                    if(control is WallpaperView) {
                        WallpaperView wallpaperView = control as WallpaperView;
                        if(sender == wallpaperView) {
                            wallpaperView.showOptions();
                        } else {
                            wallpaperView.hideOptions();
                        }
                    }
                }
                lastEntered = sender;
                flowLayoutPanel1.Focus();
            }
        }

        private void WallpaperListForm_FormClosing(object sender, FormClosingEventArgs e) {
            queue.Add(Wallpaper.EMPTY_WALL);
            data.WallpaperAdded -= wallpaperAdded;
            data.WallpaperRemoved -= wallpaperRemoved;
            data.WallpaperUpdated -= wallpaperUpdated;
        }

        private void WallpaperListForm_Shown(object sender, EventArgs e) {
            flowLayoutPanel1.AutoScrollPosition = new Point(0, 0);
        }

        private void addPanel(WallpaperView wallpaperView) {
            if(InvokeRequired) {
                Invoke(new Action<WallpaperView>(addPanel), wallpaperView);
            } else {
                if(!flowLayoutPanel1.Controls.Contains(wallpaperView)) {
                    flowLayoutPanel1.Controls.Add(wallpaperView);
                }
                flowLayoutPanel1.Controls.SetChildIndex(wallpaperView, 0);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e) {
            Wallpaper wallpaper = queue.Take();
            while(wallpaper != Wallpaper.EMPTY_WALL) {
                if(!views.ContainsKey(wallpaper.Path)) {
                    setTitle(flowLayoutPanel1.Controls.Count + "(" + queue.Count + ")");
                    WallpaperView wallpaperView = new WallpaperView(wallpaper);
                    views.Add(wallpaper.Path, wallpaperView);
                    wallpaperView.MouseEnter += wallpaperView_MouseEnter;
                    wallpaperView.MouseClick += WallpaperView_MouseClick;
                    wallpaperView.MouseDoubleClick += WallpaperView_MouseDoubleClick;
                    addPanel(wallpaperView);
                } else {
                    addPanel(views[wallpaper.Path]);
                }
                if(queue.Count == 0) {
                    setTitle(flowLayoutPanel1.Controls.Count.ToString());
                }
                wallpaper = queue.Take();
            }
        }

        private void WallpaperView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ShowView((WallpaperView)sender);
        }

        private WallpaperView displayedView = null;

        private void ShowView(WallpaperView view)
        {
            if(view == null || !view.wallpaper.Exists)
            {
                viewer.Visible = false;
                viewer.BackgroundImage = null;
            }
            else
            {
                viewer.Visible = true;
                viewer.BackgroundImage = System.Drawing.Image.FromFile(view.wallpaper.Path);
            }
        }

        private WallpaperView selectedView = null;
        private void WallpaperView_MouseClick(object sender, MouseEventArgs e)
        {
            selectView((WallpaperView)sender);
        }

        private void selectView(WallpaperView view)
        {
            if (selectedView != null)
            {
                selectedView.deselect();
            }
            if (view != null)
            {
                if (view.wallpaper.Exists)
                {
                    view.Focus();
                    view.select();
                }
                else
                {
                    view = null;
                }
            }
            selectedView = view;
        }

        private void setTitle(string p) {
            if(InvokeRequired) {
                Invoke(new Action<string>(setTitle), p);
            } else {
                this.Text = "Wallpapers:" + p;
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            panel2.Visible = !panel2.Visible;
        }

        private void button2_Click(object sender, EventArgs e) {
            data.purge();
        }

        private void button3_Click(object sender, EventArgs e) {
            data.enableWPF();
            application.AppContext.ExitThread();
        }

        private void button4_Click(object sender, EventArgs e) {
            data.DeleteThumb = !data.DeleteThumb;
            updateThumbButton();
        }

        private void updateThumbButton() {
            if(data.DeleteThumb) {
                button4.Text = "Unused Thumbnails are Deleted";
            } else {
                button4.Text = "Unused Thumbnails are not Deleted";
            }
        }

        private void viewer_Click(object sender, EventArgs e)
        {
            ShowView(null);
        }
    }
}
