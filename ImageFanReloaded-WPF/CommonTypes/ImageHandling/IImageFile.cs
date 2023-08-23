using System.Windows.Media;

namespace ImageFanReloaded.CommonTypes.ImageHandling
{
    public interface IImageFile
    {
        string FileName { get; }

		ImageSize ImageSize { get; }

		ImageSource GetImage();
		ImageSource GetResizedImage(ImageSize viewPortSize);

        void ReadImageDataFromDisc();
        ImageSource GetThumbnail();
        void DisposeImageData();
    }
}
