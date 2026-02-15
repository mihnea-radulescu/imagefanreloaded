using System.Threading.Tasks;
using ImageFanReloaded.Core.Settings;

namespace ImageFanReloaded.Core.Controls;

public interface IFolderVisualState
{
	void NotifyStopThumbnailGeneration();

	Task ClearVisualState();
	Task UpdateVisualState(ITabOptions tabOptions);

	void UpdateFolderInfoText(
		ITabOptions tabOptions,
		int previousSelectedImageFileSizeInBytes,
		int currentSelectedImageFileSizeInBytes);

	void DisposeCancellationTokenSource();
}
