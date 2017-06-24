using ImageFanReloaded.CommonTypes.Disc.Interface;
using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.Views.Interface;

namespace ImageFanReloaded.Factories.Interface
{
    public interface ITypesFactory
    {
        IDiscQueryEngine GetDiscQueryEngine();

        IImageResizer GetImageResizer();

        IImageFile GetImageFile(string filePath);

        IMainView GetMainView();

        IImageView GetImageView();
    }
}
