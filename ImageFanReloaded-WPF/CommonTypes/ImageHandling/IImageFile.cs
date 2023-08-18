using System.Drawing;
using System.Windows.Media;

namespace ImageFanReloaded.CommonTypes.ImageHandling
{
    public interface IImageFile
    {
        string FileName { get; }

        ImageSource GetImage();
        ImageSource GetResizedImage(Rectangle imageSize);

        void ReadThumbnailInputFromDisc();
        ImageSource GetThumbnail();
        void DisposeThumbnailInput();
    }
}
