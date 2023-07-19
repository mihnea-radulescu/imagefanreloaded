using ImageFanReloadedWPF.Factories.Interface;
using ImageFanReloadedWPF.Views;
using ImageFanReloadedWPF.Views.Interface;

namespace ImageFanReloadedWPF.Factories
{
    public class ImageViewFactory : IImageViewFactory
    {
        public IImageView ImageView => new ImageView();
    }
}
