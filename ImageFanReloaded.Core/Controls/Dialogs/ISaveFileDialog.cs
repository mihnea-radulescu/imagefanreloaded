using System.Threading.Tasks;

namespace ImageFanReloaded.Core.Controls.Dialogs;

public interface ISaveFileDialog
{
	Task<string?> ShowDialog(string imageFileName, string imageFolderPath, string saveFileDialogTitle);

	bool ShouldAlwaysRefreshSaveFolder { get; }
}
