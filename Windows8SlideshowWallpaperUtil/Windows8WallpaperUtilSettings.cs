using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsSlideshowWallpaperUtil;

namespace Windows8SlideshowWallpaperUtil {
    class Windows8WallpaperUtilSettings : IWallpaperUtilSettings {

        public string[] getCurrentWallpapers() {
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop\\");
            int imageCount = (int)key.GetValue("TranscodedImageCount");
            List<string> transcodedImages = new List<string>();
            for(int i = 0; i < 10 && transcodedImages.Count<imageCount; i++) {
                string keyName = String.Format("TranscodedImageCache_{0:D3}", i);
                byte[] imageBytes = getImagePathBytes((byte[])key.GetValue(keyName));
                if(imageBytes == null) {
                    continue;
                }
                string transcodedImage = Encoding.UTF8.GetString(imageBytes);
                if(transcodedImage!=null && !transcodedImages.Contains(transcodedImage)) {
                    transcodedImages.Add(transcodedImage);
                }
            }
            if (transcodedImages.Count==0) {
                byte[] imageBytes = getImagePathBytes((byte[])key.GetValue("TranscodedImageCache"));
                if (imageBytes != null){
                    string transcodedImage = Encoding.UTF8.GetString(imageBytes);
                    if (transcodedImage != null && !transcodedImages.Contains(transcodedImage)){
                        transcodedImages.Add(transcodedImage);
                    }
                }
            }
            string[] realImages = new string[transcodedImages.Count];
            int index = 0;
            foreach(string image in transcodedImages) {
                realImages[index] = image;
                index++;
            }
            return realImages;
        }

        private byte[] getImagePathBytes(byte[] original) {
            if(original == null) {
                return null;
            }
            int size = 0;
            for(int i = 24; i < original.Length || i < 544; i++, size++) {
                if(original[i] == 0 && original[i - 1] == 0) {
                    break;
                }
            }
            byte[] imageArray = new byte[size / 2];
            for(int i = 0; i < imageArray.Length; i++) {
                imageArray[i] = original[(i * 2) + 24];
            }
            return imageArray;
        }
    }
}
