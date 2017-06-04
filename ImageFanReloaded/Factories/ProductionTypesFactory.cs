using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.CommonTypes.Disc.Interface;
using ImageFanReloaded.CommonTypes.ImageHandling;
using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.Factories.Interface;
using ImageFanReloaded.Views;
using ImageFanReloaded.Views.Interface;

namespace ImageFanReloaded.Factories
{
    public class ProductionTypesFactory
        : ITypesFactory
    {
        public IDiscQueryEngine DiscQueryEngineInstance
        {
            get { return DiscQueryEngine.Instance; }
        }

        public IImageResizer ImageResizerInstance
        {
            get { return ImageResizer.Instance; }
        }

        public IMainView MainViewInstance
        {
            get { return MainView.Instance; }
        }

        public IImageFile GetImageFile(string filePath)
        {
            return new ImageFile(filePath);
        }

        public IImageView GetImageView()
        {
            return new ImageView();
        }
    }
}
