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

namespace WindowsSlideshowWallpaperUtilForms {
    public partial class WallpaperView : UserControl {
        bool active, exist, favorited;

        internal WallpaperView() {
            InitializeComponent();
        }

        private Wallpaper wallpaper;
        internal WallpaperView(Wallpaper wallpaper) {
            this.wallpaper = wallpaper;
            InitializeComponent();
            setHovers();
            this.label2.Text = wallpaper.Path;
            this.BackgroundImage = wallpaper.Thumbnail;
            this.lblDimensions.Text = wallpaper.Dimensions;
            this.lblSize.Text = wallpaper.Filesize;
            hideOptions();
            Check();
        }

        private void setHovers() {
            button1.MouseEnter += new EventHandler(HoverImages.Instance.onEnter);
            button2.MouseEnter += new EventHandler(HoverImages.Instance.onEnter);
            button4.MouseEnter += new EventHandler(HoverImages.Instance.onEnter);
            button5.MouseEnter += new EventHandler(HoverImages.Instance.onEnter);
            button6.MouseEnter += new EventHandler(HoverImages.Instance.onEnter);
            button1.MouseLeave += new EventHandler(HoverImages.Instance.onLeave);
            button2.MouseLeave += new EventHandler(HoverImages.Instance.onLeave);
            button4.MouseLeave += new EventHandler(HoverImages.Instance.onLeave);
            button5.MouseLeave += new EventHandler(HoverImages.Instance.onLeave);
            button6.MouseLeave += new EventHandler(HoverImages.Instance.onLeave);
        }


        private void Check() {
            exist = wallpaper.Exists;
            favorited = wallpaper.Favorited;
            label3.Visible = !exist;
            button1.Enabled = exist&&!favorited;
            button2.Enabled = exist;
            button4.Enabled = exist;
            button5.Enabled = exist;
            button6.Enabled = exist;
        }

        public void showOptions() {
            active = true;
            this.BackColor = System.Drawing.SystemColors.ControlDark;
            bool favorited = wallpaper.Favorited;
            button1.Visible = !favorited && button1.Enabled;
            button2.Visible = true && button2.Enabled;
            button4.Visible = true && button4.Enabled;
            label2.Visible = true;
            lblSize.Visible = true;
            lblDimensions.Visible = true;
            label4.Visible = favorited;
            button5.Visible = false;
            button6.Visible = false;
        }

        public void hideOptions() {
            active = false;
            this.BackColor = Color.Transparent;
            label2.Visible = false;
            label4.Visible = false;
            button1.Visible = false;
            button2.Visible = false;
            button4.Visible = false;
            button5.Visible = false;
            button6.Visible = false;
            label1.Visible = false;
            lblSize.Visible = false;
            lblDimensions.Visible = false;
        }

        private void WallpaperView_DoubleClick(object sender, EventArgs e) {
            if(wallpaper.Exists) {
                System.Diagnostics.Process.Start(wallpaper.Path);
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            wallpaper.favorite();
            Check();
        }

        private void button2_Click(object sender, EventArgs e) {
            button2.Visible = false;
            button5.Visible = true;
            button6.Visible = true;
            label1.Visible = true;
        }

        private void button5_Click(object sender, EventArgs e) {
            button2.Visible = true;
            button5.Visible = false;
            button6.Visible = false;
            label1.Visible = false;
            wallpaper.delete();
            Check();
        }

        private void button6_Click(object sender, EventArgs e) {
            button2.Visible = true;
            button5.Visible = false;
            button6.Visible = false;
            label1.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e) {
            if(wallpaper.Exists) {
                ShowSelectedInExplorer.FileOrFolder(wallpaper.Path);
            }
        }

        private void button3_Click(object sender, EventArgs e) {
            Clipboard.SetData(DataFormats.Text, wallpaper.Path);
        }

        internal void UpdateWallpaper() {
            bool active = this.active;
            this.lblDimensions.Text = wallpaper.Dimensions;
            this.lblSize.Text = wallpaper.Filesize;
            hideOptions();
            Check();
            if(active) { showOptions(); }
        }
    }
    class HoverImages {
        public static readonly HoverImages Instance = new HoverImages();
        Dictionary<string, Image> normalImages = new Dictionary<string, Image>();
        Dictionary<string, Image> hoverImages = new Dictionary<string, Image>();
        private HoverImages() {
            normalImages.Add("fav", WindowsSlideshowWallpaperForms.Properties.Resources.favs1);
            normalImages.Add("explore", WindowsSlideshowWallpaperForms.Properties.Resources.folder1);
            normalImages.Add("delete", WindowsSlideshowWallpaperForms.Properties.Resources.delete1);
            normalImages.Add("ok", WindowsSlideshowWallpaperForms.Properties.Resources.check1);
            normalImages.Add("cancel", WindowsSlideshowWallpaperForms.Properties.Resources.cancel1);
            hoverImages.Add("fav", WindowsSlideshowWallpaperForms.Properties.Resources.favs);
            hoverImages.Add("explore", WindowsSlideshowWallpaperForms.Properties.Resources.folder);
            hoverImages.Add("delete", WindowsSlideshowWallpaperForms.Properties.Resources.delete);
            hoverImages.Add("ok", WindowsSlideshowWallpaperForms.Properties.Resources.check);
            hoverImages.Add("cancel", WindowsSlideshowWallpaperForms.Properties.Resources.cancel);
        }
        public void onEnter(object sender, EventArgs e) {
            assign(sender, hoverImages);
        }
        public void onLeave(object sender, EventArgs e) {
            assign(sender, normalImages);
        }

        private void assign(object sender, Dictionary<string, Image> images) {
            if(sender is Button) {
                Button button = sender as Button;
                if(button.Tag is string) {
                    Image image = images[button.Tag as string];
                    if(image != null) {
                        button.BackgroundImage = image;
                    }
                }
            }
        }
    }
}
