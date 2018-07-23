using System;
using System.Diagnostics;
using System.Windows.Media;

using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.Controls;

namespace ImageFanReloaded.CommonTypes.Info
{
    [DebuggerDisplay("{ThumbnailText}")]
    public class ThumbnailInfo
    {
        public ThumbnailInfo(IImageFile imageFile)
        {
            ImageFile = imageFile ?? throw new ArgumentNullException(nameof(imageFile));

            ThumbnailImage = GlobalData.LoadingImageThumbnail;
        }

        public ThumbnailBox ThumbnailBox { get; set; }

        public IImageFile ImageFile { get; private set; }

        public ImageSource ThumbnailImage { get; set; }
        public string ThumbnailText => ImageFile.FileName;

        public void ReadThumbnailInputFromDisc()
            => ImageFile.ReadThumbnailInputFromDisc();

        public void SaveThumbnail()
        {
            ThumbnailImage = ImageFile.GetThumbnail();
            ThumbnailImage.Freeze();
        }

        public void RefreshThumbnail()
            => ThumbnailBox.RefreshThumbnail();
    }
}
