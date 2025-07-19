using Avalonia.Platform.Storage;

namespace ImageFanReloaded.Controls.Dialogs;

public class FlatpakSaveFileDialog : SaveFileDialogBase
{
	public FlatpakSaveFileDialog(IStorageProvider storageProvider)
		: base(storageProvider)
	{
	}

	public override bool ShouldAlwaysRefreshSaveFolder => true;
}
