using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.Infrastructure;
using ImageFanReloaded.Views;

namespace ImageFanReloaded.Factories;

public interface IFolderVisualStateFactory
{
    IFolderVisualState GetFolderVisualState(
        IDiscQueryEngine discQueryEngine,
        IMainView mainView,
        IVisualActionDispatcher dispatcher,
        object generateThumbnailsLockObject,
        string folderPath);
}
