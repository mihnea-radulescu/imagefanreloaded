using ImageFanReloaded.CommonTypes.Disc.Interface;
using ImageFanReloaded.CommonTypes.ImageHandling.Interface;
using ImageFanReloaded.Views.Interface;

namespace ImageFanReloaded.Factories.Interface
{
    public interface ITypesFactory
    {
        IDiscQueryEngine DiscQueryEngineInstance { get;  }
        IImageResizer ImageResizerInstance { get; }

        IMainView MainViewInstance { get; }

        IImageFile GetImageFile(string filePath);
        IImageView GetImageView();
    }
}
