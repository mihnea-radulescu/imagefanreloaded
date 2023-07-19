using ImageFanReloaded.Factories.Interface;
using ImageFanReloaded.Views;
using ImageFanReloaded.Views.Interface;

namespace ImageFanReloaded.Factories
{
    public class ImageViewFactory : IImageViewFactory
    {
        public IImageView ImageView => new ImageView();
    }
}
