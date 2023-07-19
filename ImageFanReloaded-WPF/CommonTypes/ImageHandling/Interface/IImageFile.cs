using System.Drawing;
using System.Windows.Media;

namespace ImageFanReloadedWPF.CommonTypes.ImageHandling.Interface
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
