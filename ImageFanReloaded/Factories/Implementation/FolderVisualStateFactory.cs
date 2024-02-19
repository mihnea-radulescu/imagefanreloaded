using ImageFanReloaded.CommonTypes.Disc;
using ImageFanReloaded.Controls;
using ImageFanReloaded.Infrastructure;
using ImageFanReloaded.Infrastructure.Implementation;

namespace ImageFanReloaded.Factories.Implementation;

public class FolderVisualStateFactory : IFolderVisualStateFactory
{
    public IFolderVisualState GetFolderVisualState(
		IDiscQueryEngine discQueryEngine,
			IVisualActionDispatcher dispatcher,
			IContentTabItem contentTabItem,
			string folderName,
			string folderPath)
    {
		IFolderVisualState folderVisualState = new FolderVisualState(
			discQueryEngine,
			dispatcher,
			contentTabItem,
			folderName,
			folderPath);

		return folderVisualState;
	}
}
