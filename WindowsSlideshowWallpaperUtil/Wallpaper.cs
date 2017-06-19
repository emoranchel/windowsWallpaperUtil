using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsSlideshowWallpaperUtil {
    public class Wallpaper {
        public string Path { get { return path; } }
        public Image Thumbnail {
            get {
                lock(this) {
                    if(thumbnail == null) {
                        thumbnail = createThumbnail();
                    }
                }
                return thumbnail;
            }
        }

        private readonly string path;
        private Image thumbnail;
        private WallpaperData data;
        public readonly static Wallpaper EMPTY_WALL = new Wallpaper();
        private readonly FileInfo thumbFile;
        private string filesize;
        private string dimensions;

        public string Filesize { get { return filesize; } }
        public string Dimensions { get { return dimensions; } }


        internal Wallpaper(string path, WallpaperData data) {
            this.data = data;
            this.path = path;
            FileInfo file = new FileInfo(path);
            string thumbFileName = file.Name;
            if(file.Exists) {
                filesize = formatFileSize(file.Length);
            }
            if(thumbFileName.Contains(".")) {
                thumbFileName = thumbFileName.Substring(0, thumbFileName.LastIndexOf('.'));
            }
            if(thumbFileName.Length > 96) {
                thumbFileName = thumbFileName.Substring(0, 96);
            }
            this.thumbFile = new FileInfo(".\\wallpaper-thumbs\\" + thumbFileName + ".jpg");
        }

        private string formatFileSize(long p) {
            if(p < 1024) {
                return p + "B";
            }
            if(p < (1048576)) {
                return (p / 1024) + "KB";
            }
            if(p < (1073741824)) {
                return (p / (1048576)) + "MB";
            }
            return (p / (1073741824)) + "GB";
        }

        private Wallpaper() {
        }


        private Image createThumbnail() {
            try {
                Image image = System.Drawing.Image.FromFile(thumbFile.FullName);
                System.Drawing.Image FullsizeImage = System.Drawing.Image.FromFile(path);
                dimensions = FullsizeImage.Width + "x" + FullsizeImage.Height;
                return image;
            } catch { }
            try {
                System.Drawing.Image FullsizeImage = System.Drawing.Image.FromFile(path);
                dimensions = FullsizeImage.Width + "x" + FullsizeImage.Height;
                int NewWidth = 320;
                int MaxHeight = 180;
                bool OnlyResizeIfWider = true;

                // Prevent using images internal thumbnail
                FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
                FullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

                if(OnlyResizeIfWider) {
                    if(FullsizeImage.Width <= NewWidth) {
                        NewWidth = FullsizeImage.Width;
                    }
                }

                int NewHeight = FullsizeImage.Height * NewWidth / FullsizeImage.Width;
                if(NewHeight > MaxHeight) {
                    // Resize with height instead
                    NewWidth = FullsizeImage.Width * MaxHeight / FullsizeImage.Height;
                    NewHeight = MaxHeight;
                }

                System.Drawing.Image NewImage = FullsizeImage.GetThumbnailImage(NewWidth, NewHeight, null, IntPtr.Zero);

                // Clear handle to original file so that we can overwrite it if necessary
                FullsizeImage.Dispose();

                // Save resized picture
                if(!thumbFile.Directory.Exists) {
                    thumbFile.Directory.Create();
                }
                NewImage.Save(thumbFile.FullName, ImageFormat.Jpeg);
                return NewImage;

            } catch(Exception e) {
                return Properties.Resources.NoImage;
            }

        }


        public bool Exists {
            get {
                return File.Exists(path);
            }
        }
        public bool Favorited {
            get {
                FileInfo file = new FileInfo(path);
                return File.Exists(data.FavoriteDirectory + "\\" + file.Name);
            }
        }

        public void favorite() {
            if(Exists && !Favorited) {
                if(!Directory.Exists(data.FavoriteDirectory)) {
                    Directory.CreateDirectory(data.FavoriteDirectory);
                }
                FileInfo file = new FileInfo(path);
                System.IO.File.Copy(path, data.FavoriteDirectory + "\\" + file.Name, false);
                data.onUpdate(this);
            }
        }

        public void delete() {
            if(Exists) {
                Microsoft.VisualBasic.FileIO.FileSystem.DeleteFile(path, Microsoft.VisualBasic.FileIO.UIOption.OnlyErrorDialogs, Microsoft.VisualBasic.FileIO.RecycleOption.SendToRecycleBin);
                data.onUpdate(this);
            }
        }

        public void deleteThumb() {
            if(data.DeleteThumb) {
                try { thumbFile.Delete(); } catch(Exception e) { Console.Write(e); }
            }
        }
    }
}
