using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.Controls;
using ImageFanReloaded.Infrastructure;

namespace ImageFanReloaded.Factories
{
    public interface IFolderVisualStateFactory
    {
		IFolderVisualState GetFolderVisualState(
			IDiscQueryEngine discQueryEngine,
			IVisualActionDispatcher dispatcher,
			IContentTabItem contentTabItem,
			string folderPath);
    }
}
