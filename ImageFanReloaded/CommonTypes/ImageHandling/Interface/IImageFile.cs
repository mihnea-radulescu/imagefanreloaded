using System.Drawing;
using System.Windows.Media;

namespace ImageFanReloaded.CommonTypes.ImageHandling.Interface
{
    public interface IImageFile
    {
        string FileName { get; }

        ImageSource Image { get; }

        ImageSource GetResizedImage(Rectangle imageSize);

        void ReadThumbnailInputFromDisc();
        ImageSource Thumbnail { get; }
    }
}
