using ImageFanReloaded.CommonTypes.Disc.Interface;
using ImageFanReloaded.Infrastructure.Interface;
using ImageFanReloaded.Views.Interface;

namespace ImageFanReloaded.Factories.Interface
{
    public interface IFolderVisualStateFactory
    {
        IFolderVisualState GetFolderVisualState(
            IDiscQueryEngine discQueryEngine,
            IMainView mainView,
            IVisualActionDispatcher dispatcher,
            object generateThumbnailsLockObject,
            string folderPath);
    }
}
