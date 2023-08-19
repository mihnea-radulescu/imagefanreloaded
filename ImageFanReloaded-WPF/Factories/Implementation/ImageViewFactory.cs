using ImageFanReloaded.Views;
using ImageFanReloaded.Views.Implementation;

namespace ImageFanReloaded.Factories.Implementation
{
    public class ImageViewFactory
        : IImageViewFactory
    {
        public IImageView ImageView => new ImageWindow();
    }
}
