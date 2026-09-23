using System.Threading.Tasks;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface IFolderVisualState
{
	void NotifyStopThumbnailGeneration();

	Task ClearVisualState();
	Task UpdateVisualState(ITabOptions tabOptions);

	void UpdateFolderImageFilesTotalSize(
		ITabOptions tabOptions,
		int previousSelectedImageFileSizeInBytes,
		int currentSelectedImageFileSizeInBytes);

	void SetFolderInfoText(ITabOptions tabOptions);

	void DisposeCancellationTokenSource();
}
