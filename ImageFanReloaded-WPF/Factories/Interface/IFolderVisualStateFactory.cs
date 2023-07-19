using ImageFanReloadedWPF.CommonTypes.Disc.Interface;
using ImageFanReloadedWPF.Infrastructure.Interface;
using ImageFanReloadedWPF.Views.Interface;

namespace ImageFanReloadedWPF.Factories.Interface
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
