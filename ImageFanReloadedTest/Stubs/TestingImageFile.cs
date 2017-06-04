using ImageFanReloaded;
using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Media;

namespace ImageFanReloadedTest.Stubs
{
    public class TestingImageFile
        : IImageFile
    {
        public TestingImageFile(string filePath)
        {
            FileName = Path.GetFileName(filePath);
        }

        public string FileName { get; private set; }

        public ImageSource Image
        {
            get { return GlobalData.LoadingImageThumbnail; }
        }

        public ImageSource GetResizedImage(Rectangle size)
        {
            return GlobalData.LoadingImageThumbnail;
        }

        public void ReadThumbnailInputFromDisc()
        {
        }

        public ImageSource Thumbnail
        {
            get { return GlobalData.LoadingImageThumbnail; }
        }
    }
}
