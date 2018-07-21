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
        public ProductionTypesFactory()
        {
            _discQueryEngine = new DiscQueryEngine();

            _imageResizer = new ImageResizer();

            _mainView = new MainView();
        }

        public IDiscQueryEngine GetDiscQueryEngine()
        {
            return _discQueryEngine;
        }

        public IImageResizer GetImageResizer()
        {
            return _imageResizer;
        }

        public IImageFile GetImageFile(string filePath)
        {
            return new ImageFile(_imageResizer, filePath);
        }

        public IMainView GetMainView()
        {
            return _mainView;
        }

        public IImageView GetImageView()
        {
            return new ImageView();
        }

        #region Private

        private readonly IDiscQueryEngine _discQueryEngine;

        private readonly IImageResizer _imageResizer;

        private readonly IMainView _mainView;

        #endregion
    }
}
