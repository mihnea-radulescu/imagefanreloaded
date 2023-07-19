using ImageFanReloadedWPF.CommonTypes.Disc.Interface;
using ImageFanReloadedWPF.Factories.Interface;
using ImageFanReloadedWPF.Infrastructure;
using ImageFanReloadedWPF.Infrastructure.Interface;
using ImageFanReloadedWPF.Views.Interface;

namespace ImageFanReloadedWPF.Factories
{
    public class FolderVisualStateFactory : IFolderVisualStateFactory
    {
        public IFolderVisualState GetFolderVisualState(
            IDiscQueryEngine discQueryEngine,
            IMainView mainView,
            IVisualActionDispatcher dispatcher,
            object generateThumbnailsLockObject,
            string folderPath)
        {
            var folderVisualState = new FolderVisualState(
                discQueryEngine,
                mainView,
                dispatcher,
                generateThumbnailsLockObject,
                folderPath);

            return folderVisualState;
        }
    }
}
